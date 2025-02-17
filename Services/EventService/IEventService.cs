using System.Linq.Expressions;
using YawShop.Services.EventService.Models;

namespace YawShop.Services.EventService;

public interface IEventService
{
    public Task<EventModel> CreateAsync(EventModel newEvent);

    public Task<EventModel> UpdateAsync(EventModel sourceEvent);

    public Task<EventModel> RemoveAsync(string eventCode);

    public Task<List<EventModel>> FindAsNoTrackingAsync(Expression<Func<EventModel, bool>> predicate);

    public Task<List<EventModel>> FindAsync(Expression<Func<EventModel, bool>> predicate);
    
}