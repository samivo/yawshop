using System.Linq.Expressions;
using YawShop.Services.ClientService.Models;
using YawShop.Services.ProductService.Models;

namespace YawShop.Services.ClientService;

public interface IClientService
{
    public Task CreateAsync(ClientModel client);

    public Task<List<ClientModel>> GetAsync(Expression<Func<ClientModel, bool>> predicate);

    public Task<List<ClientModel>> GetAllAsync();

    /// <summary>
    /// Validates the client model to ensure it contains at least the required fields specified in the product.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="product"></param>
    /// <returns>True if client contains fields required in the product. Otherwise false.</returns>
    public void ValidateAdditionalFields(ClientModel client, List<ProductSpesificClientFields> fieldsFromAllProductsInCart);

    public void ValidateAndSanitizeClientInput(ClientModel client);
}