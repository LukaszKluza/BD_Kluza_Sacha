using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using MongoDB.Driver;
using System.Linq.Expressions;

[Route("api/[controller]")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] Client client)
    {
        try
        {
            await _clientService.CreateClientAsync(client);
            return Ok("Client created successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the client: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClient(int id, [FromBody] Client client)
    {
        try
        {
            var success = await _clientService.UpdateClientAsync(id, client);
            if (success)
            {
                return Ok($"Client with ID '{id}' updated successfully.");
            }
            else
            {
                return NotFound("Client not found.");
            }
           
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the client: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        try
        {
            var success = await _clientService.DeleteClientAsync(id);
            if (success)
            {
                return Ok("Client deleted successfully.");
            }
            else
            {
                return NotFound("Client not found.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the client: {ex.Message}");
        }
    }
    [HttpGet("Clients")]
    public async Task<IActionResult> GetClientsPerFilterAsync(int? id = null, string? first_name = null, string? last_name = null, string? phone_number = null,
    string? gender = null, string? pesel = null, string? address = null, string? city = null, string? country = null, int? minTotal_rental_days = null, 
    int? maxTotal_rental_days = null, DateTime? minCustomerSince = null, DateTime? maxCustomerSince = null, DateTime? minBirthday = null, DateTime? maxBirthday = null)
    {
        try
        {
            var filterDefinitioinBuilder = Builders<Client>.Filter;
            var filter = Builders<Client>.Filter.Empty;

            if(id.HasValue){
                filter &= filterDefinitioinBuilder.Eq(client => client._id, id.Value);
            }
            if(!string.IsNullOrWhiteSpace(first_name)){
                filter &= filterDefinitioinBuilder.Eq(client => client.First_Name, first_name);
            }
            if(!string.IsNullOrWhiteSpace(last_name)){
                filter &= filterDefinitioinBuilder.Eq(client => client.Last_Name, last_name);
            }
            if(!string.IsNullOrWhiteSpace(phone_number)){
                filter &= filterDefinitioinBuilder.Eq(client => client.Phone_Number, phone_number);
            }
            if(!string.IsNullOrWhiteSpace(gender)){
                filter &= filterDefinitioinBuilder.Eq(client => client.Gender, gender);
            }
            if(!string.IsNullOrWhiteSpace(pesel)){
                filter &= filterDefinitioinBuilder.Eq(client => client.Pesel, pesel);
            }
            if(!string.IsNullOrWhiteSpace(address)){
                filter &= filterDefinitioinBuilder.Eq(client => client.Address, address);
            }
            if(!string.IsNullOrWhiteSpace(city)){
                filter &= filterDefinitioinBuilder.Eq(client => client.City, city);
            }
            if(!string.IsNullOrWhiteSpace(country)){
                filter &= filterDefinitioinBuilder.Eq(client => client.Country, country);
            }
            filter &= filterDefinitioinBuilder.Gte(client => client.Total_Rental_Days, minTotal_rental_days ?? 0);
            filter &= filterDefinitioinBuilder.Lte(client => client.Total_Rental_Days, maxTotal_rental_days ?? int.MaxValue);

            filter &= AddDateRangeFilter(filter, filterDefinitioinBuilder, client => client.Customer_Since, minCustomerSince, maxCustomerSince);
            filter &= AddDateRangeFilter(filter, filterDefinitioinBuilder, Client => Client.Birth_Day, minBirthday, maxBirthday);
            
            var result = await _clientService.GetClientsPerFilterAsync(filter);
            if (result.Any())
            {
                return Ok(result);
            }
            else{
                return NotFound("Clients not found.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving clients: {ex.Message}");
        }
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

    private static FilterDefinition<T> AddDateRangeFilter<T>(
        FilterDefinition<T> filter,
        FilterDefinitionBuilder<T> filterBuilder,
        Expression<Func<T, DateTime?>> field,
        DateTime? minValue,
        DateTime? maxValue)
        {
            if (minValue.HasValue)
            {
                filter &= filterBuilder.Gte(field, minValue.Value);
            }
            if (maxValue.HasValue)
            {
                filter &= filterBuilder.Lte(field, maxValue.Value);
            }
            return filter;
        }
}