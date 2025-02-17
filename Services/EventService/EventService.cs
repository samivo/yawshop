using System.Linq.Expressions;
using AngleSharp.Dom.Events;
using Microsoft.EntityFrameworkCore;
using YawShop.Attributes;
using YawShop.Services.ClientService;
using YawShop.Services.EventService.Models;
using YawShop.Services.ProductService;
using YawShop.Services.StockService;
using YawShop.Utilities;

namespace YawShop.Services.EventService;

public class EventService : IEventService
{

    private readonly ILogger<EventService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IClientService _client;

    public EventService(ILogger<EventService> logger, ApplicationDbContext context, IClientService clientService )
    {
        _logger = logger;
        _context = context;
        _client = clientService;
    }

    public async Task<EventModel> CreateAsync(EventModel newEvent)
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

            //Ensure event code is assigned
            newEvent.Code = Guid.NewGuid().ToString();

            await _context.Events.AddAsync(newEvent);
            await _context.SaveChangesAsync();

            return newEvent;
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

            foreach (var singleEvent in events)
            {
                if (singleEvent.ClientCode != null)
                {
                    singleEvent.Client = (await _client.GetAsync(client => client.Code == singleEvent.ClientCode)).FirstOrDefault();
                }
            }

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

            foreach (var singleEvent in events)
            {
                if (singleEvent.ClientCode != null)
                {
                    singleEvent.Client = (await _client.GetAsync(client => client.Code == singleEvent.ClientCode)).FirstOrDefault();
                }
            }


            return events;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get event(s): {err}", ex.ToString());
            throw;
        }
    }

    public async Task<EventModel> RemoveAsync(string eventCode)
    {
        try
        {
            //Remove not allowed if event has registrations and is upcoming event
            var evnt = await _context.Events.SingleAsync(e => e.Code == eventCode);

            if (evnt.ClientCode != null)
            {
                throw new InvalidOperationException("Can not remove event that has registrations. Please cancel registrations first.");
            }

            _context.Remove(evnt);

            await _context.SaveChangesAsync();

            return evnt;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to remove event: {err}", ex.ToString());
            throw;
        }
    }

    public async Task<EventModel> UpdateAsync(EventModel updatedEvent)
    {
        try
        {
            var oldEvent = await _context.Events.SingleAsync(e => e.Code == updatedEvent.Code);

            PropertyCopy.CopyWithoutAttribute(updatedEvent, oldEvent, typeof(NoApiUpdateAttribute));

            await _context.SaveChangesAsync();
            return oldEvent;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update event: {err}", ex.ToString());
            throw;
        }
    }


}