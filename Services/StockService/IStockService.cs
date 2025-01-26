using YawShop.Services.CheckoutService.Models;

namespace YawShop.Services.StockService;

public interface IStockService
{
    /// <summary>
    /// Updates product quantities and set giftcards and discounts used if AddQuantities is true.
    /// If AddQuantities is false, reduce product quantities and set giftcards and discounts unused.
    /// </summary>
    /// <param name="checkoutCode">Checkout model id</param>
    /// <param name="AddQuantities">True to increase and false to decrease quantities. </param>
    /// <returns></returns>
    public Task UpdateQuantitiesAsync(string checkoutCode, bool AddQuantities);

    /// <summary>
    /// Updates product quantities and set giftcards and discounts used if AddQuantities is true.
    /// If AddQuantities is false, reduce product quantities and set giftcards and discounts unused.
    /// </summary>
    /// <param name="checkoutModel"></param>
    /// <param name="AddQuantities"></param>
    /// <returns></returns>
    public Task UpdateQuantitiesAsync(CheckoutModel checkoutModel, bool AddQuantities);

}