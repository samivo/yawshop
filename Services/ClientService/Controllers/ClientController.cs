using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YawShop.Services.ClientService;
using YawShop.Services.ClientService.Models;

namespace YawShop.Services.ClientService.Controllers;

[ApiController]
[Route("/api/v1/client/")]
public class ClientController : ControllerBase
{
    private readonly ILogger<ClientController> _logger;
    private readonly IClientService _client;

    public ClientController(ILogger<ClientController> logger, IClientService clientService)
    {
        _logger = logger;
        _client = clientService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetClients()
    {
        try
        {
            var clients = await _client.GetAllAsync();
            return Ok(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get clients: {err}", ex.ToString());
            return BadRequest($"Failed to get clients: {ex.Message}");
        }
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateClient([FromBody] ClientModel client)
    {
        try
        {
            await _client.CreateAsync(client);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create client: {err}", ex.ToString());
            return BadRequest($"Failed to create client: {ex.Message}");
        }
    }


}