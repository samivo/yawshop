using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YawShop.Services.CheckoutService;
using YawShop.Services.EventService;
using YawShop.Services.EventService.Models;
using YawShop.Services.StockService;

namespace YawShop.Services.EventService.Controllers;


[ApiController]
[Route("/api/v1/event/")]
public class EventController : ControllerBase
{
    private readonly ILogger<EventController> _logger;
    private readonly IEventService _event;
    private readonly IStockService _stock;
    private readonly ICheckoutService _checkout;

    public EventController(ILogger<EventController> logger, IEventService eventService, IStockService stockService, ICheckoutService checkoutService)
    {
        _logger = logger;
        _event = eventService;
        _stock = stockService;
        _checkout = checkoutService;
    }


    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var events = await _event.FindAsNoTrackingAsync(e => true);
            return Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError("Unable to get events: {err}", ex.ToString());
            return StatusCode(400, $"No events found.");
        }
    }

    [AllowAnonymous]
    [HttpGet("public")]
    public async Task<IActionResult> PublicGetAll()
    {
        try
        {   
            //Find all visible events
            var events = await _event.FindAsNoTrackingAsync(e => e.IsVisible);

            var responseObject = new List<object>();

            foreach (var evnt in events)
            {
                responseObject.Add(evnt.Public());
            }

            return Ok(responseObject);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get events: {err}", ex.ToString());
            return StatusCode(400, "No events found.");
        }
    }

    [HttpPost("")]
    public async Task<IActionResult> Create([FromBody] EventModel newEvent)
    {
        try
        {
           var createdEvent = await _event.CreateAsync(newEvent);
            return StatusCode(201,createdEvent );
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create event: {err}", ex.ToString());
            return StatusCode(400, $"Unable to create event.");
        }
    }

    [HttpPut("")]
    public async Task<IActionResult> Update([FromBody] EventModel newEvent)
    {
        try
        {
            //Check is event unregistration needed?
            var oldEvent = (await _event.FindAsNoTrackingAsync(e => e.Code == newEvent.Code)).Single();

            //If client code is nulled then unregister
            if (oldEvent.ClientCode != null && newEvent.ClientCode == null)
            {
                //Client should be unregistered
                if (oldEvent.Client != null)
                {
                    //Get correct checkout and pass it to stock service
                    var checkout = (await _checkout.FindAsync(c => c.ClientId == oldEvent.Client.Id))?.Single();

                    if (checkout != null)
                    {   
                        //Do not cancel event, if there is ongoing payment process.
                        if(checkout.PaymentStatus != CheckoutService.Models.PaymentStatus.Initialized)
                        {
                            await _stock.UpdateQuantitiesAsync(checkout, false, oldEvent.Code);
                        }
                        else
                        {
                            throw new InvalidOperationException("Cannot unregister event because there is ongoing payment process.");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("No checkout found for event?");
                    }

                }
                else
                {
                    throw new InvalidOperationException("Client should be unregistered, but event does not contain client?");
                }

            }

            var evnt = await _event.UpdateAsync(newEvent);

            return Ok(evnt);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update event: {err}", ex.ToString());
            return StatusCode(400, $"Unable to update event.");
        }
    }


    [HttpDelete("{eventCode}")]
    public async Task<IActionResult> Delete(string eventCode)
    {
        try
        {
            await _event.RemoveAsync(eventCode);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to remove event: {err}", ex.ToString());
            return StatusCode(500, $"Unable to remove event: {ex.Message}");
        }
    }

}