using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YawShop.Services.DiscountService;
using YawShop.Services.DiscountService.Models;

namespace YawShop.Services.DiscountService.Controllers;


[ApiController]
[Route("/api/v1/discount/")]
public class DiscountController : ControllerBase
{
    private readonly ILogger<DiscountController> _logger;
    private readonly IDiscountService _discount;

    public DiscountController(ILogger<DiscountController> logger, IDiscountService discountService)
    {
        _logger = logger;
        _discount = discountService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var discounts = await _discount.FindAsNoTrackingAsync(discount => true);
            return Ok(discounts);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get discounts: {err}", ex.ToString());
            return BadRequest($"Failed to get discounts: {ex.Message}");
        }
    }


    [HttpPost("")]
    public async Task<IActionResult> CreateDiscount([FromBody] DiscountModel discountModel)
    {
        try
        {
            await _discount.CreateAsync(discountModel);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to fetch discount: {err}", ex.ToString());
            return BadRequest($"Failed to fetch discount: {ex.Message}");
        }
    }

    [HttpPut("{discountCode}")]
    public async Task<IActionResult> UpdateDiscount(string discountCode, [FromBody] DiscountModel discountModel)
    {
        try
        {
            await _discount.UpdateAsync(discountCode, discountModel);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update discount: {err}", ex.ToString());
            return BadRequest($"Failed to update discount: {ex.Message}");
        }
    }

    [HttpDelete("{discountCode}")]
    public async Task<IActionResult> DeleteDiscount(string discountCode)
    {
        try
        {
            await _discount.RemoveAsync(discountCode);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to delete discount: {err}", ex.ToString());
            return StatusCode(400, $"Failed to delete discount: {ex.Message}");
        }
    }

    public class ValidateBody
    {
        [MaxLength(50)]
        public required string DiscountCode { get; set; }
        public required string[] ProductCodes { get; set; }
    }

    [AllowAnonymous]
    [HttpPost("validate/public")]
    public async Task<IActionResult> Validate([FromBody] ValidateBody body)
    {
        try
        {
            //Get discounts
            var discounts = await _discount.FindAsNoTrackingAsync(discount => discount.Code == body.DiscountCode);

            //Should only find one
            if(discounts.Count > 1){
                throw new InvalidOperationException("Multiple discounts found?");
            }

            if (discounts == null || discounts.Count == 0)
            {
                return StatusCode(400, "Invalid code.");
            }

            var discount = discounts.First();

            if (!string.Equals(discount.Code.ToUpper(), body.DiscountCode.ToUpper()))
            {
                return StatusCode(400, "Invalid code.");
            }

            if (discount.IsValid() && body.ProductCodes.Contains(discount.TargetProductCode))
            {
                return Ok(discount.Public());
            }

            return StatusCode(400, "Invalid code.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error with discount: {err}", ex.ToString());
            return StatusCode(400, "Invalid code.");
        }
    }


}