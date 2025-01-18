using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YawShop.Services.GiftcardService.Models;

namespace YawShop.Services.GiftcardService.Controllers;


[ApiController]
[Route("/api/v1/giftcard/")]
public class GiftcardController : ControllerBase
{
    private readonly ILogger<GiftcardController> _logger;

    private readonly IGiftcardService _giftcard;

    public GiftcardController(ILogger<GiftcardController> logger, IGiftcardService giftcardService)
    {
        _logger = logger;
        _giftcard = giftcardService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var giftcards = await _giftcard.FindAsNoTrackingAsync(g => true);
            return Ok(giftcards);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Giftcard create error: {err}", ex.ToString());
            return StatusCode(400, "Unable to find giftcards.");
        }
    }

    [AllowAnonymous]
    [HttpGet("public/{giftcardCode}")]
    public async Task<IActionResult> GetPublic(string giftcardCode)
    {
        /*
        This is public endpoint! Use public() method to exclude properties mark as [notPublic] attribute
        */

        try
        {
            var giftcard = (await _giftcard.FindAsNoTrackingAsync(giftcard => giftcard.Code == giftcardCode)).Single();

            return Ok(giftcard.Public());
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Failed to get giftcard {err}", ex.ToString());
            return StatusCode(400, "No giftcard found.");
        }
    }


    [HttpPost("")]
    public async Task<IActionResult> Create([FromBody] GiftcardModel giftcardModel)
    {
        try
        {
            await _giftcard.CreateAsync(giftcardModel);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Giftcard create error: {err}", ex.ToString());
            return StatusCode(500, "Unable to create new giftcard.");
        }
    }

    [HttpPut("{giftcardCode}")]
    public async Task<IActionResult> Update([FromBody] GiftcardModel giftcardModel)
    {
        try
        {
            await _giftcard.UpdateAsync(giftcardModel.Code, giftcardModel);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Giftcard update error: {err}", ex.ToString());
            return StatusCode(500, "Unable to update new giftcard.");
        }
    }

    [HttpPost("sendEmail/{giftcardCode}")]
    public async Task<IActionResult> SendEmail(string giftcardCode)
    {
        try
        {
            await _giftcard.SendGiftcardEmailAsync(giftcardCode);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Giftcard email failed: {}", ex.ToString());
            return StatusCode(500, "Sending giftcard to email failed.");
        }
    }


    [HttpDelete("{giftcardCode}")]
    public async Task<IActionResult> Delete(string giftcardCode)
    {
        try
        {
            await _giftcard.DeleteAsync(giftcardCode);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Giftcard delete failed: {}", ex.ToString());
            return StatusCode(500, "Giftcard delete failed.");
        }
    }



}
