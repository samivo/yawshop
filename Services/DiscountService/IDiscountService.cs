using System.Linq.Expressions;
using YawShop.Services.DiscountService.Models;

namespace YawShop.Services.DiscountService;

public interface IDiscountService
{
    public Task CreateAsync(DiscountModel discountModel);

    public Task<List<DiscountModel>> FindAsNoTrackingAsync(Expression<Func<DiscountModel, bool>> predicate);

    public Task<List<DiscountModel>> FindAsync(Expression<Func<DiscountModel, bool>> predicate);

    public Task RemoveAsync(string discountCode);

    public Task UpdateAsync(string discountCode, DiscountModel discountModel);

    /// <summary>
    /// sums the value to discounts quantity used
    /// </summary>
    /// <param name="discountCode"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Task AddQuantityUsed(string discountCode, int value);
}
