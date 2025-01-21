using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;
using YawShop.Services.CheckoutService.Models;

namespace YawShop.Services.CheckoutService.Controllers;


[ApiController]
[Route("/api/v1/checkout")]
public class CheckoutController : ControllerBase
{
    private readonly ILogger<CheckoutController> _logger;
    private readonly ICheckoutService _checkout;

    public CheckoutController(ILogger<CheckoutController> logger, ICheckoutService checkoutService)
    {
        _logger = logger;
        _checkout = checkoutService;

    }
    
    [AllowAnonymous]
    [HttpPost("public")]
    public async Task<IActionResult> CreatePayment([FromBody] ShoppingCartModel shoppingCart)
    {
        try
        {
            var href = await _checkout.ProcessCart(shoppingCart);
            return Ok(JsonSerializer.Serialize(new { href }));
        }
        catch (Exception ex)
        {
            _logger.LogError("Paytrail payment creation error: {err}", ex.ToString());
            return StatusCode(400);
        }
    }
}