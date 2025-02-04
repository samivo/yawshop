using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
            //Get all products
            var products = await _product.FindAsNoTrackingAsync(p => true);

            if(products.Count == 0)
            {
                return NotFound("No products.");

            }

            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Failed to get products: {err}", ex.ToString());
            return StatusCode(500, "Failed to get products.");
        }
    }

    [AllowAnonymous]
    [HttpGet("public")]
    public async Task<IActionResult> GetAllPublic()
    {
        try
        {
            //Get all products that are active and visible to public
            var products = await _product.FindAsNoTrackingAsync(product => product.IsActive && product.IsVisibleToPublic);

            var publicProducts = new List<ProductModelPublic>();

            //Add only public product properties to the respond object
            foreach (var product in products)
            {
                publicProducts.Add(_product.GetPublicProduct(product));
            }

            return Ok(publicProducts);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get public products: {err}", ex.ToString());
            return NotFound("No products.");
        }
    }

    [AllowAnonymous]
    [HttpGet("public/{productCode}")]
    public async Task<IActionResult> GetAllPublicByCode(string productCode)
    {
        try
        {
            //Get product by code that is active and visible to public
            var product = await _product.FindAsNoTrackingAsync(product => product.Code == productCode && product.IsActive && product.IsVisibleToPublic);

            //Check that there is only one product with the code
            if (product.Count == 0)
            {
                return NotFound("Product not found.");
            }

            if(product.Count > 1)
            {
                _logger.LogCritical("Duplicate product code found: {productCode}", productCode);
                return NotFound("Product not found.");
            }

            //Create respond object with only public properties

            return Ok(_product.GetPublicProduct(product.First()));
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get public product: {err}", ex.ToString());
            return NotFound("Product not found.");
        }
    }

    [HttpPost("")]
    public async Task<IActionResult> Create([FromBody] ProductModel product)
    {
        try
        {
            var prod = await _product.CreateAsync(product);

            return Ok(prod);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Failed to create product: {err}", ex.ToString());
            return StatusCode(400, $"Failed to create product: {ex.Message} ");
        }
    }

    [HttpPut("")]
    public async Task<IActionResult> Update([FromBody] ProductModel product)
    {
        try
        {
           var prod = await _product.UpdateAsync(product);
            return Ok(prod);
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
            var prod = await _product.RemoveAsync(productCode);
            return Ok(prod);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Failed to remove product: {err}", ex.ToString());
            return StatusCode(400, "Failed to remove product.");
        }
    }


}