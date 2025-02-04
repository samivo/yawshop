using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using YawShop.Services.CheckoutService.Models;
using YawShop.Services.ClientService;
using YawShop.Services.DiscountService;
using YawShop.Services.EmailService;
using YawShop.Services.EventService;
using YawShop.Services.EventService.Models;
using YawShop.Services.GiftcardService;
using YawShop.Services.GiftcardService.Models;
using YawShop.Services.PaymentService;
using YawShop.Services.ProductService;
using YawShop.Services.ProductService.Models;
using YawShop.Services.StockService;
using YawShop.Utilities;

namespace YawShop.Services.CheckoutService;

public class CheckoutService : ICheckoutService
{
    private readonly ILogger<CheckoutService> _logger;
    private readonly IClientService _client;
    private readonly IProductService _product;
    private readonly IGiftcardService _giftcard;
    private readonly IDiscountService _discount;
    private readonly IEventService _event;
    private readonly ApplicationDbContext _context;
    private readonly IPaymentService _payment;
    private readonly IStockService _stock;
    private readonly IEmailer _email;


    public CheckoutService(ILogger<CheckoutService> logger, IClientService clientService, IProductService productService, IGiftcardService giftcardService, IDiscountService discountService, ApplicationDbContext applicationDbContext, IEventService eventService, IPaymentService paymentService, IStockService stockService, IEmailer emailer)
    {
        _logger = logger;
        _client = clientService;
        _product = productService;
        _giftcard = giftcardService;
        _discount = discountService;
        _context = applicationDbContext;
        _event = eventService;
        _payment = paymentService;
        _stock = stockService;
        _email = emailer;
    }



    public async Task<CheckoutModel?> GetSingleAsync(Expression<Func<CheckoutModel, bool>> predicate)
    {
        try
        {
            var checkout = await _context.Checkouts.AsNoTracking().Include(c => c.Products).SingleOrDefaultAsync(predicate);
            return checkout;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get checkout: {err}", ex.ToString());
            throw;
        }
    }

    /// <summary>
    /// Processes the cart and init payment
    /// </summary>
    /// <param name="cart"></param>
    /// <returns> Link to payment gateway</returns>
    public async Task<string> ProcessCart(ShoppingCartModel cart)
    {
        await _context.Database.BeginTransactionAsync();

        try
        {
            await VerifyShoppingCartAsync(cart);

            var checkoutObject = await CreateCheckoutObjectsync(cart);

            var productCodes = checkoutObject.Products.Select(p => p.ProductCode);
            var ListOfProductFields = (await _product.FindAsNoTrackingAsync(product => productCodes.Contains(product.Code))).Select(p => p.CustomerFields);

            var cartFieldList = new List<ProductSpesificClientFields>();

            //Get all fields from each product in cart. Maybe some other solution for foreach inside foreach
            foreach (var FieldsInProduct in ListOfProductFields)
            {
                if (FieldsInProduct == null)
                {
                    continue;
                }
                foreach (var field in FieldsInProduct)
                {
                    cartFieldList.Add(field);
                }
            }

            _client.ValidateAdditionalFields(cart.Client, cartFieldList);

            _client.ValidateAndSanitizeClientInput(cart.Client);

            //Save customer
            await _client.CreateAsync(checkoutObject.Client);
            checkoutObject.ClientId = checkoutObject.Client.Id;

            //save checkout
            await _context.Checkouts.AddAsync(checkoutObject);
            await _context.SaveChangesAsync();

            //To prevent race conditions. Still not atomic? TODO transactions and optimistic concurrency
            await VerifyShoppingCartAsync(cart);

            await _stock.UpdateQuantitiesAsync(checkoutObject, true);

            string? href = "";

            if (checkoutObject.TotalAmount > 0)
            {
                href = await _payment.CreateAsync(checkoutObject);

                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
            }
            else
            {
                //Final amount zero or less -> handle ok payment. TODO: change success redict link
                href = "success";
                checkoutObject.PaymentStatus = PaymentStatus.Ok;

                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();

                await ProcessSuccessfulPayment(checkoutObject.Reference);
            }

            return href;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to process shopping cart: {err}", ex.ToString());
            await _context.Database.RollbackTransactionAsync();
            throw;
        }
    }

    /// <summary>
    /// Verifies shopping cart product availability, giftcards and discounts.
    /// </summary>
    /// <param name="cart"></param>
    /// <returns></returns>
    public async Task VerifyShoppingCartAsync(ShoppingCartModel cart)
    {
        /*Shopping cart should contain a list of distinct products. Quantity is used if same product purchased multiple times.
          If product is event, product can have a list of distinct event codes.
          If multiple events (event codes) are listed in shopping cart, product quantity should match with number of events.
          Each event is handled as invividual product in checkout.
        */

        try
        {
            //Card should not contain duplicated items. Quantity per item should be used.
            var distinctItems = cart.ProductDetails.DistinctBy(p => p.ProductCode);

            if (distinctItems.Count() != cart.ProductDetails.Count)
            {
                throw new InvalidOperationException("Shoppingcart contains duplicate products.");
            }

            //Product should not contain duplicated events
            foreach (var product in cart.ProductDetails)
            {
                if (product.EventCodes != null)
                {
                    var distinctEvents = product.EventCodes.Distinct();

                    if (distinctEvents.Count() != product.EventCodes.Count)
                    {
                        throw new InvalidOperationException("Shoppingcart product(s) contains duplicated event codes.");
                    }
                }

            }

            //Check each product availability and quantities
            foreach (var productDetail in cart.ProductDetails)
            {
                var product = (await _product.FindAsNoTrackingAsync(product => product.Code == productDetail.ProductCode)).SingleOrDefault() ?? throw new InvalidOperationException("Cannot find product with given code.");

                //change to exception type?
                if (!_product.IsAvailable(product))
                {
                    throw new InvalidOperationException("Product is not available.");
                }

                //check there is enough products left
                if (product.QuantityLeft != null && product.QuantityLeft <= 0)
                {
                    throw new InvalidOperationException($"Not enough products available ({product.Name}).");
                }

                if (product.ProductType == ProductType.Event)
                {
                    if (productDetail.EventCodes == null)
                    {
                        throw new InvalidOperationException("Shopping cart contains product that is event type, but event codes are missing?");
                    }

                    if (productDetail.EventCodes.Count != productDetail.Quantity)
                    {
                        throw new InvalidOperationException("Shopping cart event code quantity do not match with product quantity.");
                    }

                    foreach (var eventCode in productDetail.EventCodes)
                    {
                        var evnt = (await _event.FindAsNoTrackingAsync(evnt => evnt.Code == eventCode)).SingleOrDefault() ?? throw new InvalidOperationException("No event found with given code.");

                        if (evnt.Status != EventStatus.Available)
                        {
                            throw new InvalidOperationException("Event in shopping cart is not available.");
                        }
                    }
                }

            }


            //Combine codes to one or create service for this?

            if (!string.IsNullOrEmpty(cart.GiftcardCode))
            {
                var giftcard = (await _giftcard.FindAsNoTrackingAsync(giftcard => giftcard.Code == cart.GiftcardCode)).SingleOrDefault() ?? throw new InvalidOperationException($"Cant find any giftcards with code {cart.GiftcardCode}");
                if (!giftcard.IsValid())
                {
                    throw new InvalidOperationException("Giftcard in cart is not valid.");
                }
            }

            if (!string.IsNullOrEmpty(cart.DiscountCode))
            {
                var discount = (await _discount.FindAsNoTrackingAsync(discount => discount.Code == cart.DiscountCode)).SingleOrDefault() ?? throw new InvalidOperationException($"No discount found with code {cart.DiscountCode}");

                if (!discount.IsValid())
                {
                    throw new InvalidOperationException($"Discount code {cart.DiscountCode} in cart is not valid. ");
                }

                if (!cart.ProductDetails.Any(p => p.ProductCode == discount.TargetProductCode))
                {
                    throw new InvalidOperationException("Discount code is not targeted any of products in cart.");
                }
            }


            return;
        }
        catch (Exception ex)
        {
            _logger.LogError("Shopping cart verification error. {err}", ex.ToString());
            throw;
        }
    }

    public async Task HandlePaymentCallbackAsync(CallbackResult callbackResult)
    {
        await _context.Database.BeginTransactionAsync();
        try
        {
            var checkout = await _context.Checkouts.SingleOrDefaultAsync(checkout => checkout.Reference == callbackResult.CheckoutReference);

            if (checkout == null)
            {
                throw new InvalidOperationException($"Payment callback reference is invalid. Cannot find any checkout with refrence: {callbackResult.CheckoutReference}");
            }

            if (callbackResult.PaymentStatus == PaymentStatus.Initialized || callbackResult.PaymentStatus == PaymentStatus.New)
            {
                throw new InvalidOperationException($"Payment status in callback can't be {checkout.PaymentStatus}");
            }

            else if (callbackResult.PaymentStatus == PaymentStatus.Fail || callbackResult.PaymentStatus == PaymentStatus.Cancelled)
            {
                if (callbackResult.PaymentStatus == PaymentStatus.Fail)
                {
                    //Commented out, payment fail comes if customer just cancels payment process
                    //_logger.LogCritical("Payment failed. Checkout reference: {checkoutReference}. Transaction id: {transactionId}", callbackResult.CheckoutReference, callbackResult.TransactionId);
                }
                else
                {
                    _logger.LogInformation("Payment cancelled. Checkout reference: {checkoutReference}", callbackResult.CheckoutReference);
                }
                await _stock.UpdateQuantitiesAsync(checkout.Reference, false);
            }

            else if (callbackResult.PaymentStatus == PaymentStatus.Pending || callbackResult.PaymentStatus == PaymentStatus.Delayed)
            {
                //Log and do nothing. Ok or failed should come later.
                _logger.LogInformation("Payment status in callback was {status}", callbackResult.PaymentStatus);
            }

            else if (callbackResult.PaymentStatus == PaymentStatus.Ok)
            {
                //Handle multiple ok requests
                if (checkout.PaymentStatus != PaymentStatus.Ok)
                {
                    await ProcessSuccessfulPayment(callbackResult.CheckoutReference);
                }
            }

            else
            {
                throw new InvalidOperationException("Payment status not handled. PaymentStatuse class contains more statuses than callback can handle?");
            }

            checkout.PaymentStatus = callbackResult.PaymentStatus;
            checkout.UpdatetAt = DateTime.Now;

            await _context.SaveChangesAsync();

            await _context.Database.CommitTransactionAsync();

            return;

        }
        catch (Exception)
        {
            await _context.Database.RollbackTransactionAsync();
            throw;
        }
    }

    /// <summary>
    /// Process successful payment. Update discount and giftcard quantities.
    /// Send invoice
    /// Create giftcards
    /// </summary>
    /// <returns></returns>
    public async Task ProcessSuccessfulPayment(string checkoutReference)
    {
        try
        {

            var checkout = await _context.Checkouts.Include(checkout => checkout.Products).SingleAsync(checkout => checkout.Reference == checkoutReference);
            checkout.Client = (await _client.GetAsync(client => client.Id == checkout.ClientId)).SingleOrDefault() ?? throw new InvalidOperationException("Cant find checkout's user from database?");

            //Send Receipt
            try
            {
                var receipt = new EmailMessage
                {
                    Body = ReceiptTemplate.GetEmailBody(checkout),
                    Subject = "Kuitti",
                    To = [checkout.Client.Email]
                };

                await _email.SendMailAsync(receipt);
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Payment success but failed to send receipt to customer: {ex}", ex.ToString());
            }

            foreach (var productInfo in checkout.Products)
            {
                var product = (await _product.FindAsNoTrackingAsync(p => p.Code == productInfo.ProductCode)).SingleOrDefault();

                //Skip discounts and giftcards in checkout list
                if (product == null)
                {
                    continue;
                }

                if (product.ProductType == ProductType.Giftcard)
                {
                    for (int i = 0; i < productInfo.Units; i++)
                    {
                        var giftcard = new GiftcardModel
                        {
                            ExpireDate = DateTime.Now.AddDays(product.GiftcardPeriodInDays),
                            Name = productInfo.ProductName,
                            PurchaseDate = DateTime.Now,
                            TargetProductCode = product.GiftcardTargetProductCode,
                            OwnerClientId = checkout.ClientId
                        };

                        await _giftcard.CreateAsync(giftcard);

                        var giftcardEmail = new EmailMessage
                        {
                            Body = GiftcardEmail.GetEmailBody(giftcard.Code),
                            Subject = "Lahjakortti",
                            To = new List<string>{
                                checkout.Client.Email
                            }
                        };

                        await _email.SendMailAsync(giftcardEmail);
                    }
                }

                if (product.ProductType == ProductType.Event)
                {
                    var evnt = (await _event.FindAsNoTrackingAsync(e => e.Code == productInfo.EventCode)).SingleOrDefault() ?? throw new InvalidOperationException("Checkout contains event that not exists.");
                    var eventEmail = new EmailMessage
                    {
                        Body = EventEmail.GetEmailBody(product.Name, evnt.EventStart, product.Code),
                        Subject = "Varausvahvistus",
                        To = new List<string>{
                        checkout.Client.Email
                        }
                    };

                    //Make unified email sender that checks unsended emails some interval?
                    await _email.SendMailAsync(eventEmail);
                }

            }

            return;
        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// Creates unified checkout object from shopping cart to payment provider.
    /// </summary>
    /// <param name="cart"></param>
    /// <returns></returns>
    private async Task<CheckoutModel> CreateCheckoutObjectsync(ShoppingCartModel cart)
    {
        try
        {
            var checkOutObject = new CheckoutModel
            {
                Client = cart.Client,
                Products = new List<CheckoutItem>()
            };

            int TotalAmount = 0;

            foreach (var productInfo in cart.ProductDetails)
            {
                var product = (await _product.FindAsNoTrackingAsync(p => p.Code == productInfo.ProductCode)).SingleOrDefault() ?? throw new InvalidOperationException("Cant find product by code.");

                //Handle events individually
                if (product.ProductType == ProductType.Event)
                {

                    if (productInfo.EventCodes == null)
                    {
                        throw new InvalidOperationException("Shopping cart contains product that is event type, but event codes are missing?");
                    }

                    foreach (var eventCode in productInfo.EventCodes)
                    {
                        var evnt = (await _event.FindAsNoTrackingAsync(evnt => evnt.Code == eventCode)).SingleOrDefault() ?? throw new InvalidOperationException("No event found with given code");
                        var dateString = evnt.EventStart.ToString("yyyy-MM-dd HH:mm");

                        checkOutObject.Products.Add(new CheckoutItem
                        {
                            UnitPrice = product.PriceInMinorUnitsIncludingVat,
                            Units = 1,
                            VatPercentage = product.VatPercentage,
                            ProductCode = product.Code,
                            ProductName = product.Name + $" {dateString}",
                            EventCode = eventCode,
                        });

                        TotalAmount += product.PriceInMinorUnitsIncludingVat;
                    }
                }
                else
                {
                    checkOutObject.Products.Add(new CheckoutItem
                    {
                        UnitPrice = product.PriceInMinorUnitsIncludingVat,
                        Units = productInfo.Quantity,
                        VatPercentage = product.VatPercentage,
                        ProductCode = product.Code,
                        ProductName = product.Name
                    });

                    TotalAmount += product.PriceInMinorUnitsIncludingVat * productInfo.Quantity;
                }
            }

            //Apply giftcard
            if (!string.IsNullOrEmpty(cart.GiftcardCode))
            {
                var giftcard = (await _giftcard.FindAsNoTrackingAsync(giftcard => giftcard.Code == cart.GiftcardCode)).SingleOrDefault() ?? throw new InvalidOperationException("No giftcard found with given code");

                var giftcardTargetProduct = (await _product.FindAsNoTrackingAsync(product => product.Code == giftcard.TargetProductCode)).SingleOrDefault() ?? throw new InvalidOperationException("No product found with giftcard's target product code");

                //If giftcard not value based, discount whole product price.
                if (!giftcard.IsValueBased)
                {
                    giftcard.ValueLeftInMinorUnits = giftcardTargetProduct.PriceInMinorUnitsIncludingVat;
                }

                checkOutObject.Products.Add(new CheckoutItem
                {
                    ProductName = (await _product.FindAsNoTrackingAsync(product => product.GiftcardTargetProductCode == giftcard.TargetProductCode)).First().Name,
                    ProductCode = giftcard.Code,
                    UnitPrice = -giftcard.ValueLeftInMinorUnits,
                    Units = 1,
                    VatPercentage = giftcardTargetProduct.VatPercentage
                });

                TotalAmount -= giftcard.ValueLeftInMinorUnits;

            }

            //Apply discount
            if (!string.IsNullOrEmpty(cart.DiscountCode))
            {

                var discount = (await _discount.FindAsNoTrackingAsync(discount => discount.Code == cart.DiscountCode)).SingleOrDefault() ?? throw new InvalidOperationException("No discount found with given code");
                var discountProduct = (await _product.FindAsNoTrackingAsync(product => product.Code == discount.TargetProductCode)).SingleOrDefault() ?? throw new InvalidOperationException("No product found with given code.");

                checkOutObject.Products.Add(new CheckoutItem
                {
                    ProductName = discountProduct.Name,
                    ProductCode = discount.Code,
                    UnitPrice = -discount.DiscountAmountInMinorUnits,
                    Units = 1,
                    VatPercentage = discountProduct.VatPercentage
                });

                TotalAmount -= discount.DiscountAmountInMinorUnits;
            }

            if (TotalAmount < 0)
            {
                checkOutObject.TotalAmount = 0;
            }
            else
            {
                checkOutObject.TotalAmount = TotalAmount;
            }


            return checkOutObject;

        }
        catch (Exception ex)
        {
            _logger.LogError("Checkout object generation error: {err}", ex.ToString());
            throw;
        }
    }


}