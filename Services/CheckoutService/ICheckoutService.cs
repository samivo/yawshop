using System.Linq.Expressions;
using YawShop.Services.CheckoutService.Models;

namespace YawShop.Services.CheckoutService;

public interface ICheckoutService
{
    /// <summary>
    /// Finds checkouts matching predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>List of checkoutmodels</returns>
    public Task<List<CheckoutModel>> GetAsync(Expression<Func<CheckoutModel, bool>> predicate);

    /// <summary>
    /// Updates checkout by transaction id.
    /// </summary>
    /// <param name="transactionId"></param>
    /// <param name="paymentStatus"></param>
    /// <returns></returns>
    public Task HandlePaymentCallbackAsync(CallbackResult callbackResult);

    /// <summary>
    /// Finds single or default checkoutmodel
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>Checkoutmodel</returns>
    public Task<CheckoutModel?> GetSingleAsync(Expression<Func<CheckoutModel, bool>> predicate);

    /// <summary>
    /// Process shopping cart and return href to payment gateway
    /// </summary>
    /// <param name="cart"></param>
    /// <returns></returns>
    public Task<string> ProcessCart(ShoppingCartModel cart);

}
