using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YawShop.Services.ProductService.Models;

namespace YawShop.Services.ProductService.Controllers;


[ApiController]
[Route("/api/v1/product/")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly IProductService _product;

    public ProductController(ILogger<ProductController> logger, IProductService productService)
    {
        _logger = logger;
        _product = productService;
    }


    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var products = await _product.FindAsNoTrackingAsync(p => true);

            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Failed to get products: {err}", ex.ToString());
            return StatusCode(400, "No products.");
        }
    }

    [AllowAnonymous]
    [HttpGet("public")]
    public async Task<IActionResult> GetAllPublic()
    {
        try
        {
            var products = await _product.FindAsNoTrackingAsync(product => product.IsActive);

            var respondObject = new List<object>();

            foreach (var product in products)
            {
                if (product.IsVisibleToPublic)
                {
                    respondObject.Add(product.Public());
                }
            }

            return Ok(respondObject);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get public products: {err}", ex.ToString());
            return StatusCode(400, "No products found.");
        }
    }

    [HttpPost("")]
    public async Task<IActionResult> Create([FromBody] ProductModel product)
    {
        try
        {
            await _product.CreateAsync(product);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Failed to create product: {err}", ex.ToString());
            return StatusCode(400, $"Failed to create product: {ex.Message} ");
        }
    }

    [HttpPut("{productCode}")]
    public async Task<IActionResult> Update(string productCode, [FromBody] ProductModel product)
    {
        try
        {
            await _product.UpdateAsync(productCode, product);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Failed to update product: {err}", ex.ToString());
            return StatusCode(500, "Failed to update product.");
        }
    }

    [HttpDelete("{productCode}")]
    public async Task<IActionResult> Delete(string productCode)
    {
        try
        {
            await _product.RemoveAsync(productCode);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Failed to remove product: {err}", ex.ToString());
            return StatusCode(400, "Failed to remove product.");
        }
    }


}