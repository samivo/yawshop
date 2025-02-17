using Microsoft.EntityFrameworkCore;
using YawShop.Services.CheckoutService.Models;
using YawShop.Services.Database;
using YawShop.Services.DiscountService;
using YawShop.Services.EventService;
using YawShop.Services.GiftcardService;
using YawShop.Services.ProductService;

namespace YawShop.Services.StockService;

public class StockService : IStockService
{
    private readonly ILogger<StockService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IGiftcardService _giftcard;
    private readonly IDiscountService _discount;
    private readonly IProductService _product;
    private readonly IEventService _event;

    public StockService(ILogger<StockService> logger, ApplicationDbContext context, IGiftcardService giftcardService, IDiscountService discountService, IProductService productService, IEventService eventService)
    {
        _logger = logger;
        _context = context;
        _giftcard = giftcardService;
        _discount = discountService;
        _product = productService;
        _event = eventService;
    }

    public async Task UpdateQuantitiesAsync(CheckoutModel checkoutModel, bool addQuantities, string? eventToUnregister = null)
    {
        try
        {
            //Get product and event codes from checkout
            var productCodes = checkoutModel.Products.Select(p => p.ProductCode).ToList();
            var eventCodes = checkoutModel.Products.Select(p => p.EventCode).ToList();

            //Get giftcards and discountcodes from checkout
            var giftcardCodes = checkoutModel.Products.Select(p => p.GiftcardCode).ToList();
            var discountCodes = checkoutModel.Products.Select(p => p.DiscountCode).ToList();

            //If event should be unregistered
            if (eventToUnregister != null)
            {
                //Get only spesific codes which is related to the event to unregister

                productCodes = checkoutModel.Products.Where(p => p.EventCode == eventToUnregister).Select(p => p.ProductCode).ToList();
                eventCodes = [eventToUnregister];

                giftcardCodes = checkoutModel.Products.Where(p => p.EventCode == eventToUnregister).Select(p => p.GiftcardCode).ToList();
                discountCodes = checkoutModel.Products.Where(p => p.EventCode == eventToUnregister).Select(p => p.DiscountCode).ToList();
            }

            //Get real product and event models by codes
            var products = await _product.FindAsync(p => productCodes.Contains(p.Code));
            var events = await _event.FindAsync(p => eventCodes.Contains(p.Code));
            

            //Get giftcards
            var giftcards = await _giftcard.FindAsync(giftcard => giftcardCodes.Contains(giftcard.Code));

            //Get discounts
            var discounts = await _discount.FindAsync(discount => discountCodes.Contains(discount.Code));

            foreach (var product in products)
            {
                //Get quantity per product from checkout object
                var quantity = checkoutModel.Products.Where(p => p.ProductCode == product.Code).Select(p => p.Units).Single();

                //Sum checkout quantities to original product quantities
                if (addQuantities)
                {   
                    product.QuantityUsed += quantity;
                }
                else
                {
                    product.QuantityUsed -= quantity;
                }
            }

            //Update event registrations. Currently, only one registrations per event.
            foreach (var singleEvent in events)
            {

                if (addQuantities)
                {
                    //Increase quantities. Client code added later, so quantities are not necessary anymore?
                    singleEvent.ClientCode = checkoutModel.Client.Code;
                }
                else
                {
                    //Sets 
                    singleEvent.ClientCode = null;
                }
            }

            //Update giftcards
            foreach (var giftcard in giftcards)
            {
                if (addQuantities)
                {
                    giftcard.SetUsed(checkoutModel.ClientId);
                }
                else
                {
                    giftcard.SetUnused();
                }
            }

            //Update discounts
            foreach (var discount in discounts)
            {
                if (addQuantities)
                {
                    discount.QuantityUsed += 1;
                }
                else
                {
                    discount.QuantityUsed -= 1;
                }
            }

            await _context.SaveChangesAsync();

            //jos rekisteröinnin poisto tarkasta onko maksu ok, jos on, mitä sitten?
        }
        catch (Exception ex)
        {

            _logger.LogError("Failed to update product and event quantities: {err}", ex.ToString());
            throw;
        }
    }
}