using System.Linq.Expressions;
using YawShop.Services.EventService.Models;

namespace YawShop.Services.EventService;

public interface IEventService
{
    public Task CreateAsync(EventModel newEvent);

    public Task UpdateAsync(string eventCode, EventModel sourceEvent);

    public Task RemoveAsync(string eventCode);

    public Task<List<EventModel>> FindAsNoTrackingAsync(Expression<Func<EventModel, bool>> predicate);

    public Task<List<EventModel>> FindAsync(Expression<Func<EventModel, bool>> predicate);


    /// <summary>
    /// sums the value to event registrations used
    /// </summary>
    /// <param name="eventCode"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Task AddRegistration(string eventCode, int value);
}