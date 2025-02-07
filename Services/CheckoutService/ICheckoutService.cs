using System.Linq.Expressions;
using YawShop.Services.CheckoutService.Models;

namespace YawShop.Services.CheckoutService;

public interface ICheckoutService
{

    /// <summary>
    /// Updates checkout by transaction id.
    /// </summary>
    /// <param name="transactionId"></param>
    /// <param name="paymentStatus"></param>
    /// <returns></returns>
    public Task HandlePaymentCallbackAsync(CallbackResult callbackResult);

    public Task<List<CheckoutModel>?> FindAsync(Expression<Func<CheckoutModel, bool>> predicate);

    /// <summary>
    /// Process shopping cart and return href to payment gateway
    /// </summary>
    /// <param name="cart"></param>
    /// <returns></returns>
    public Task<string> ProcessCart(ShoppingCartModel cart);

}