using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;


[Route("api/[controller]")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly IConfiguration _configuration;

    public ClientController(IClientService clientService, IConfiguration configuration)
    {
        _clientService = clientService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateClient([FromBody] Register register_model)
    {
        try
        {
            var existingUser = await _clientService.GetUserByEmailAsync(register_model.Email);
            if (existingUser != null)
            {
                return BadRequest("User already exists");
            }

            var client = new Client
            {
                First_Name = register_model.First_Name,
                Last_Name = register_model.Last_Name,
                Phone_Number = register_model.Phone_Number,
                Gender = register_model.Gender,
                Birth_Day = register_model.Birth_Day,
                Pesel = register_model.Pesel,
                Email = register_model.Email,
                Address = register_model.Address,
                City = register_model.City,
                Country = register_model.Country,
                Customer_Since = DateTime.Now.Date,
                Total_Rental_Days = 0,
                Password_Hash = BCrypt.Net.BCrypt.HashPassword(register_model.Password)
            };
            
             await _clientService.CreateClientAsync(client);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating client: {ex.Message}");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login login_model)
    {
        var client = await _clientService.GetUserByEmailAsync(login_model.Email);
        if (client == null || !BCrypt.Net.BCrypt.Verify(login_model.Password, client.Password_Hash))
        {
            return Unauthorized();
        }

        var token = GenerateJwtToken(client);
        return Ok(new { Token = token });
    }

    private string GenerateJwtToken(Client client)
    {
        var secretKey = Convert.ToBase64String(CreateRandomKey(32));
        var issuer = "your_issuer";
        var audience = "your_audience";

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, client.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    
    private byte[] CreateRandomKey(int bytes)
    {
        using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
        {
            var key = new byte[bytes];
            rng.GetBytes(key);
            return key;
        }
    }
}