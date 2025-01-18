using YawShop.Services.CheckoutService.Models;

namespace YawShop.Services.PaymentService;

public interface IPaymentService
{
    /// <summary>
    /// Initiates a new payment and sets transaction id to checkout object.
    /// </summary>
    /// <param name="checkoutModel"></param>
    /// <returns>Link to payment providers payment portal</returns>
    public Task<string> CreateAsync(CheckoutModel checkoutModel);

    /// <summary>
    /// Validates the payment callback or webhook and extract data to callback result.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Return payment status</returns>
    public CallbackResult ValidateCallbackOrWebhook(HttpRequest request);


}