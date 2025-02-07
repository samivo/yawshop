using YawShop.Services.CheckoutService.Models;

namespace YawShop.Services.StockService;

public interface IStockService
{

    /// <summary>
    /// Updates product quantities and set giftcards and discounts used if AddQuantities is true.
    /// If AddQuantities is false, reduce product quantities and set giftcards and discounts unused.
    /// </summary>
    /// <param name="checkoutModel"></param>
    /// <param name="AddQuantities"></param>
    /// <returns></returns>
    public Task UpdateQuantitiesAsync(CheckoutModel checkoutModel, bool AddQuantities);

}