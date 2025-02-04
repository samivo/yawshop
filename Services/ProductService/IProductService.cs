using System.Linq.Expressions;
using YawShop.Services.ProductService.Models;

namespace YawShop.Services.ProductService;

public interface IProductService
{

    public Task<ProductModel> CreateAsync(ProductModel product);

    public Task<ProductModel> RemoveAsync(string productCode);

    public Task<ProductModel> UpdateAsync(ProductModel product);

    public Task<List<ProductModel>> FindAsNoTrackingAsync(Expression<Func<ProductModel, bool>> predicate);

    public Task<List<ProductModel>> FindAsync(Expression<Func<ProductModel, bool>> predicate);

    public bool IsAvailable(ProductModel product);

    /// <summary>
    /// sums the value to quantity used. Negative value decreases quantity.
    /// </summary>
    /// <param name="productCode"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Task AddQuantityUsed(string productCode, int value);

    public ProductModelPublic GetPublicProduct(ProductModel product);

}