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

    public async Task UpdateQuantitiesAsync(string checkoutReference, bool AddQuantities)
    {
        try
        {
            //Get checkout object by code
            var checkout = await _context.Checkouts.Include(c => c.Products).SingleAsync(c => c.Reference == checkoutReference);

            await UpdateQuantitiesAsync(checkout, AddQuantities);

        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update product and event quantities: {err}", ex.ToString());
            
            throw;
        }
    }

    public async Task UpdateQuantitiesAsync(CheckoutModel checkoutModel, bool AddQuantities)
    {
        try
        {
            //checkoutmode.products are checkoutItems
            var itemCodes = checkoutModel.Products.Select(p => p.ProductCode).ToList();
            var eventCodes = checkoutModel.Products.Select(p => p.EventCode).ToList();

            //Get giftcards and discountcodes from checkout
            var giftcardCodes = checkoutModel.Products.Select(p => p.GiftcardCode).ToList();
            var discountCodes = checkoutModel.Products.Select(p => p.DiscountCode).ToList();

            //Get real product and event models by codes
            var products = await _product.FindAsync(p => itemCodes.Contains(p.Code));
            var events = await _event.FindAsync(p => eventCodes.Contains(p.Code));

            //Get giftcards
            var giftcards = await _giftcard.FindAsync(giftcard => giftcardCodes.Contains(giftcard.Code));

            //Get discounts
            var discounts = await _discount.FindAsync(discount => discountCodes.Contains(discount.Code));

            foreach (var product in products)
            {
                //Get quantity per product from checkout object
                var quantity = checkoutModel.Products.Where(p => p.ProductCode == product.Code).Select(p => p.Units).Single();

                //Sum checkout quantities  to original product quantities
                if (AddQuantities)
                {
                    product.QuantityUsed += quantity;
                }
                else
                {
                    product.QuantityUsed -= quantity;
                }
            }

            //Update event quantities
            foreach (var singleEvent in events)
            {
                var quantity = checkoutModel.Products.Where(p => p.EventCode == singleEvent.Code).Select(p => p.Units).Single();

                if (AddQuantities)
                {
                    singleEvent.RegistrationsQuantityUsed += quantity;
                }
                else
                {
                    singleEvent.RegistrationsQuantityUsed -= quantity;
                }
            }

            //Update giftcards
            foreach (var giftcard in giftcards)
            {
                if (AddQuantities)
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
                if (AddQuantities)
                {
                    discount.QuantityUsed += 1;
                }
                else
                {
                    discount.QuantityUsed -= 1;
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {

            _logger.LogError("Failed to update product and event quantities: {err}", ex.ToString());
            throw;
        }
    }
}