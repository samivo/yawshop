using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using YawShop.Attributes;
using YawShop.Services.EventService.Models;
using YawShop.Services.ProductService;
using YawShop.Utilities;

namespace YawShop.Services.EventService;

public class EventService : IEventService
{

    private readonly ILogger<EventService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IProductService _product;

    public EventService(ILogger<EventService> logger, ApplicationDbContext context, IProductService productService)
    {
        _logger = logger;
        _context = context;
        _product = productService;
    }

    public async Task AddRegistration(string eventCode, int value)
    {
        try
        {
            var evnt = await _context.Events.SingleAsync(e => e.Code == eventCode);
            evnt.RegistrationsQuantityUsed += value;

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to add registration to event: {err}", ex.ToString());
            throw;
        }
    }

    public async Task CreateAsync(EventModel newEvent)
    {
        try
        {

            if (await _context.Events.AsNoTracking().AnyAsync(e => e.Code == newEvent.Code))
            {
                throw new InvalidOperationException("Failed to create event. Dupclicate event code found.");
            }

            if (!await _context.Products.AsNoTracking().AnyAsync(p => p.Code == newEvent.ProductCode))
            {
                throw new InvalidOperationException("Failed to create event. No product found with given product code.");
            }

            await _context.Events.AddAsync(newEvent);
            await _context.SaveChangesAsync();

            return;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create event: {err}", ex.ToString());
            throw;
        }
    }

    public async Task<List<EventModel>> FindAsync(Expression<Func<EventModel, bool>> predicate)
    {
        try
        {
            var events = await _context.Events.Where(predicate).ToListAsync();

            return events;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get event(s): {err}", ex.ToString());
            throw;
        }
    }

    public async Task<List<EventModel>> FindAsNoTrackingAsync(Expression<Func<EventModel, bool>> predicate)
    {
        try
        {
            var events = await _context.Events.AsNoTracking().Where(predicate).ToListAsync();

            return events;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get event(s): {err}", ex.ToString());
            throw;
        }
    }

    public async Task RemoveAsync(string eventCode)
    {
        try
        {
            //Remove not allowed if event has registrations and is upcoming event
            var evnt = await _context.Events.SingleAsync(e => e.Code == eventCode);

            if (evnt.RegistrationsQuantityUsed > 0)
            {
                throw new InvalidOperationException("Can not remove event that has registrations. Please cancel registrations first.");
            }

            _context.Remove(evnt);

            await _context.SaveChangesAsync();

            return;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to remove event: {err}", ex.ToString());
            throw;
        }
    }

    public async Task UpdateAsync(string eventCode, EventModel updatedEvent)
    {
        try
        {
            var oldEvent = await _context.Events.SingleAsync(e => e.Code == eventCode);

            PropertyCopy.CopyWithoutAttribute(updatedEvent, oldEvent, typeof(NoApiUpdateAttribute));

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update event: {err}", ex.ToString());
            throw;
        }
    }

}