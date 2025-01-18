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
    private readonly IStockService _stock;

    //test
    private readonly ApplicationDbContext _context;
    private readonly IClientService _client;

    public PaymentController(ILogger<PaymentController> logger, ICheckoutService checkoutService, IPaymentService paymentService, IStockService stockService, ApplicationDbContext applicationDbContext, IClientService clientService)
    {
        _logger = logger;
        _checkout = checkoutService;
        _payment = paymentService;
        _stock = stockService;
        _context = applicationDbContext;
        _client = clientService;
    }

    [AllowAnonymous]
    [HttpGet("callback")]
    public async Task<IActionResult> PaymentCallbackGet()
    {
        try
        {
            var callbackResult = _payment.ValidateCallbackOrWebhook(HttpContext.Request);

            await _checkout.HandlePaymentCallbackAsync(callbackResult);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Payment callback error: {err}", ex.ToString());
            return StatusCode(400);
        }
    }

}
