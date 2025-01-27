using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YawShop.Services.CheckoutService;
using YawShop.Services.StockService;
using YawShop.Services.ClientService;

namespace YawShop.Services.PaymentService.Controllers;


[ApiController]
[Route("/api/v1/payment/")]
public class PaymentController : ControllerBase
{
    private readonly ILogger<PaymentController> _logger;
    private readonly ICheckoutService _checkout;
    private readonly IPaymentService _payment;

    public PaymentController(ILogger<PaymentController> logger, ICheckoutService checkoutService, IPaymentService paymentService, IStockService stockService, ApplicationDbContext applicationDbContext, IClientService clientService)
    {
        _logger = logger;
        _checkout = checkoutService;
        _payment = paymentService;
    }

    
    [AllowAnonymous]
    [HttpGet("callback")]
    public async Task<IActionResult> PaymentCallbackGet()
    {
        try
        {
            var callbackResult = _payment.ValidateCallbackOrWebhook(HttpContext.Request);

            //Since callback is validated, Ok should be returned.
            //TODO: If payment handler fails, it should tried again later?
            try
            {   
                await _checkout.HandlePaymentCallbackAsync(callbackResult);
            }
            catch (System.Exception ex) 
            {
                _logger.LogCritical("The payment callback was valid, but something went wrong while processing the payment: {ex}", ex);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Payment callback validation error: {err}", ex);
            return StatusCode(400, "Invalid callback");
        }
    }

}
