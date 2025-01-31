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

    public CheckoutController(
        ILogger<CheckoutController> logger, 
        ICheckoutService checkoutService)
    {
        _logger = logger;
        _checkout = checkoutService;
    }
    
    [HttpGet("all")]
    [Authorize]
    public async Task<IActionResult> GetAllCheckouts()
    {
        try 
        {
            _logger.LogInformation("GetAllCheckouts called by user {User}", User?.Identity?.Name ?? "unknown");
            
            if (!User?.Identity?.IsAuthenticated ?? true)
            {
                _logger.LogWarning("Unauthorized access attempt to GetAllCheckouts");
                return Unauthorized();
            }

            _logger.LogInformation("Fetching checkouts from service");
            var checkouts = await _checkout.GetAsync(c => true);
            _logger.LogInformation($"Service returned {checkouts?.Count ?? 0} checkouts");
            
            if (checkouts == null)
            {
                _logger.LogWarning("Checkout service returned null");
                return StatusCode(500, "Internal server error");
            }
            
            if (!checkouts.Any())
            {
                _logger.LogInformation("No checkouts found in database");
                return Ok(new List<CheckoutModel>());
            }
            
            _logger.LogInformation($"Returning {checkouts.Count} checkouts to client");
            return Ok(checkouts);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in GetAllCheckouts: {error}", ex.ToString());
            return StatusCode(500, "Internal server error");
        }
    }

    [AllowAnonymous]
    [HttpPost("public")]
    public async Task<IActionResult> CreatePayment([FromBody] ShoppingCartModel shoppingCart)
    {
        try
        {
            var href = await _checkout.ProcessCart(shoppingCart);
            return Ok(new { href });
        }
        catch (Exception ex)
        {
            _logger.LogError("Paytrail payment creation error: {err}", ex.ToString());
            return StatusCode(400);
        }
    }
}
