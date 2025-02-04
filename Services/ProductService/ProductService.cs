using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
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

    public async Task<ProductModel> CreateAsync(ProductModel product)
    {
        try
        {
            //Assign new product code
            product.Code = Guid.NewGuid().ToString();

            //Check there is no duplicates
            if (await _context.Products.AnyAsync(p => p.Code == product.Code))
            {
                throw new InvalidOperationException("Duplicate product found. Cant create product.");
            }

            //Add product
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;
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

    public async Task<ProductModel> RemoveAsync(string productCode)
    {
        try
        {
            //Get target product
            var product =  (await FindAsync(p => p.Code == productCode)).Single();

            _context.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to remove product: {err}", ex.ToString());
            throw;
        }
    }

    public async Task<ProductModel> UpdateAsync(ProductModel updatedProduct)
    {
        try
        {
            //Get target product
            var oldProduct = (await FindAsync(p => p.Code == updatedProduct.Code)).Single();

            //Copy allowed properties
            PropertyCopy.CopyWithoutAttribute(updatedProduct, oldProduct, typeof(NoApiUpdateAttribute));
            await _context.SaveChangesAsync();

            return oldProduct;
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
            return false;
        }

        if (product.AvailableFrom > DateTime.Now)
        {
            return false;
        }

        if (product.AvailableTo < DateTime.Now)
        {
            return false;
        }

        return true;
    }

    public async Task AddQuantityUsed(string productCode, int value)
    {
        try
        {
            var product = (await FindAsync(p => p.Code == productCode)).Single();

            product.QuantityUsed += value;
            await _context.SaveChangesAsync();

            return;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to add quantity used: {err}", ex.ToString());
            throw;
        }
    }

    public ProductModelPublic GetPublicProduct(ProductModel product)
    {
        var publicProduct = new ProductModelPublic
        {
            Code = product.Code,
            AvatarImage = product.AvatarImage,
            CustomerFields = product.CustomerFields?.Select(field => new ProductSpesificClientFieldsPublic
            {
                FieldName = field.FieldName,
                FieldType = field.FieldType,
                Href = field.Href,
                IsRequired = field.IsRequired,
            }).ToList() ?? new List<ProductSpesificClientFieldsPublic>(),
            DescriptionOrInnerHtml = product.DescriptionOrInnerHtml,
            GiftcardPeriodInDays = product.GiftcardPeriodInDays,
            GiftcardTargetProductCode = product.GiftcardTargetProductCode,
            MaxQuantityPerPurchase = product.MaxQuantityPerPurchase,
            Name = product.Name,
            PriceInMinorUnitsIncludingVat = product.PriceInMinorUnitsIncludingVat,
            ProductGroupId = product.ProductGroupId,
            ProductType = product.ProductType,
            QuantityLeft = product.QuantityLeft,
            ShortDescription = product.ShortDescription,
            VatPercentage = product.VatPercentage

        };

        return publicProduct;
    }
}
