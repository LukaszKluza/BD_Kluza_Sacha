using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;
using Microsoft.Extensions.Configuration;


[Route("api/[controller]")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;
    private IConfiguration _config { get; }

    public ClientController(IClientService clientService, IConfiguration config)
    {
        _clientService = clientService;
        _config = config;
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
    public async Task<IActionResult> UpdateClient(string id, [FromBody] Client client)
    {
        try
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest("Invalid ObjectId format.");
            }
            var success = await _clientService.UpdateClientAsync(objectId, client);
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
    public async Task<IActionResult> DeleteClient(string id)
    {
        try
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest("Invalid ObjectId format.");
            }
            var success = await _clientService.DeleteClientAsync(objectId);
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
    public async Task<IActionResult> GetClientsPerFilterAsync(string? id = null, string? first_name = null, string? last_name = null, string? phone_number = null,
    string? gender = null, string? pesel = null, string? address = null, string? city = null, string? country = null, int? minTotal_rental_days = null, 
    int? maxTotal_rental_days = null, DateTime? minCustomerSince = null, DateTime? maxCustomerSince = null, DateTime? minBirthday = null, DateTime? maxBirthday = null)
    {
        try
        {
            var filterDefinitioinBuilder = Builders<Client>.Filter;
            var filter = Builders<Client>.Filter.Empty;


            if (!string.IsNullOrEmpty(id))
            {
                if (!ObjectId.TryParse(id, out ObjectId objectId))
                {
                    return BadRequest("Invalid ObjectId format.");
                }
                else
                {
                    filter &= filterDefinitioinBuilder.Eq("_id", objectId);
                } 

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

        var jwtSettings = _config.GetSection("Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
               new Claim(ClaimTypes.NameIdentifier, client._id.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"])),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token =  tokenHandler.WriteToken(securityToken);
        return Ok(token);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetUserProfile()
    {
        try
        {
            var filterDefinitioinBuilder = Builders<Client>.Filter;
            var filter = Builders<Client>.Filter.Empty;

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found in token.");
            }

            var userId = userIdClaim.Value;
            if(!ObjectId.TryParse(userId, out ObjectId objectId)) {
                return BadRequest("Invalid ObjectId format.");
            }
            filter &= filterDefinitioinBuilder.Eq(client => client._id, objectId);
            
            var clients = await _clientService.GetClientsPerFilterAsync(filter);
            var client = clients.SingleOrDefault();
            return Ok(client);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving user profile: {ex.Message}");
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