using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using YawShop.Attributes;
using YawShop.Services.ProductService.Models;
using YawShop.Utilities;

namespace YawShop.Services.ProductService;

public class ProductService : IProductService
{

    private readonly ILogger<ProductService> _logger;
    private readonly ApplicationDbContext _context;

    public ProductService(ILogger<ProductService> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task CreateAsync(ProductModel product)
    {
        try
        {
            if (await _context.Products.AnyAsync(p => p.Code == product.Code))
            {
                throw new InvalidOperationException("Duplicate product found. Cant create product.");
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create product: {err}", ex.ToString());
            throw;
        }
    }



    public async Task<List<ProductModel>> FindAsNoTrackingAsync(Expression<Func<ProductModel, bool>> predicate)
    {
        try
        {
            var product = await _context.Products.Include(p => p.CustomerFields).AsNoTracking().Where(predicate).ToListAsync();

            return product;

        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get product(s): {err}", ex.ToString());
            throw;
        }
    }

    public async Task<List<ProductModel>> FindAsync(Expression<Func<ProductModel, bool>> predicate)
    {
        try
        {
            var product = await _context.Products.Include(p => p.CustomerFields).Where(predicate).ToListAsync();

            return product;

        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get product(s): {err}", ex.ToString());
            throw;
        }
    }

    public async Task RemoveAsync(string productCode)
    {
        try
        {
            var product = await _context.Products.SingleAsync(p => p.Code == productCode);

            //Check if product has transaction first
            //TODO
            _context.Remove(product);

            await _context.SaveChangesAsync();
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to remove product: {err}", ex.ToString());
            throw;
        }
    }

    public async Task UpdateAsync(string productCode, ProductModel updatedProduct)
    {
        try
        {
            var oldProduct = await _context.Products.Include(product => product.CustomerFields).SingleAsync(p => p.Code == productCode);

            PropertyCopy.CopyWithoutAttribute(updatedProduct, oldProduct, typeof(NoApiUpdateAttribute));

            await _context.SaveChangesAsync();
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update product: {err}", ex.ToString());
            throw;
        }
    }

    public bool IsAvailable(ProductModel product)
    {

        if (!product.IsActive)
        {
            _logger.LogInformation("Product is not available: product is inactive.");
            return false;
        }

        if (product.AvailableFrom > DateTime.Now)
        {
            _logger.LogInformation("Product is not available: sales not yes started.");
            return false;
        }

        if (product.AvailableTo < DateTime.Now)
        {
            _logger.LogInformation("Product is not available: sales ended");
            return false;
        }

        return true;
    }

    public async Task AddQuantityUsed(string productCode, int value)
    {
        try
        {
            var product = await _context.Products.SingleAsync(p => p.Code == productCode);
            product.QuantityUsed += value;

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to add quantity used: {err}", ex.ToString());
            throw;
        }
    }


}
