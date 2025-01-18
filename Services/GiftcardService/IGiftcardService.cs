using System.Linq.Expressions;
using YawShop.Services.GiftcardService.Models;

namespace YawShop.Services.GiftcardService;

public interface IGiftcardService
{
    public Task<List<GiftcardModel>> FindAsNoTrackingAsync(Expression<Func<GiftcardModel, bool>> predicate);

    public Task<List<GiftcardModel>> FindAsync(Expression<Func<GiftcardModel, bool>> predicate);

    public Task CreateAsync(GiftcardModel giftcard);

    public Task UpdateAsync(string giftcardCode, GiftcardModel giftcard);

    public Task DeleteAsync(string giftcardCode);

    public Task SendGiftcardEmailAsync(string giftcardCode);

}