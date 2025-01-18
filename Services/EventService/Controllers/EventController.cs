using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YawShop.Services.EventService;
using YawShop.Services.EventService.Models;

namespace YawShop.Services.EventService.Controllers;


[ApiController]
[Route("/api/v1/event/")]
public class EventController : ControllerBase
{
    private readonly ILogger<EventController> _logger;
    private readonly IEventService _event;


    public EventController(ILogger<EventController> logger, IEventService eventService)
    {
        _logger = logger;
        _event = eventService;
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
            var events = await _event.FindAsNoTrackingAsync(e => true);

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
            await _event.CreateAsync(newEvent);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create event: {err}", ex.ToString());
            return StatusCode(400, $"Unable to create event.");
        }
    }

    [HttpPut("{eventCode}")]
    public async Task<IActionResult> Update(string eventCode, [FromBody] EventModel newEvent)
    {
        try
        {
            await _event.UpdateAsync(eventCode, newEvent);
            return Ok();
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