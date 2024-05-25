using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MongoDB.Driver;


[Route("api/[controller]")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly IClientService _carService;

    public ClientController(IClientService clientService)
    {
        _carService = clientService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] Client client)
    {
        try
        {
            await _carService.CreateClientAsync(client);
            return Ok("Client created successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the client: {ex.Message}");
        }
    }
}