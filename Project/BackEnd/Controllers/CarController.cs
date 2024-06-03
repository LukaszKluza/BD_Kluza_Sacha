using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MongoDB.Driver;
using MongoDB.Bson;


[Route("api/[controller]")]
[ApiController]
public class CarController : ControllerBase
{
    private readonly ICarService _carService;

    public CarController(ICarService carService)
    {
        _carService = carService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCar([FromBody] Car car)
    {
        try
        {
            await _carService.CreateCarAsync(car);
            return Ok("Car created successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the car: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCar(string id, [FromBody] Car car)
    {
        try
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest("Invalid ObjectId format.");
            }
            var success = await _carService.UpdateCarAsync(objectId, car);
            if (success)
            {
                return Ok($"Car with ID '{id}' updated successfully.");
            }
            else
            {
                return NotFound("Car not found.");
            }
           
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the car: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCar(string id)
    {
        try
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest("Invalid ObjectId format.");
            }
            var success = await _carService.DeleteCarAsync(objectId);
            if (success)
            {
                return Ok("Car deleted successfully.");
            }
            else
            {
                return NotFound("Car not found.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the car: {ex.Message}");
        }
    }
    
    [HttpGet("Cars")]
    public async Task<IActionResult> GetCarsPerFilterAsync(string? modelId = null, int? seats = null, string? type = null, string? color = null, 
    int? minPower = null, int? maxPower = null, int? minCurrMileage = null, int? maxCurrMileage = null,
    double? minPricePerDay = null, double? maxPricePerDay = null, bool? isAvailable = null, int? minProductionYear = null, int? maxProductionYear = null)
    {
        try
        {
            var filterDefinitioinBuilder = Builders<Car>.Filter;
            var filter = Builders<Car>.Filter.Empty;

            if (!string.IsNullOrEmpty(modelId))
            {
                if (!ObjectId.TryParse(modelId, out ObjectId objectModelId))
                {
                    return BadRequest("Invalid ObjectId format.");
                }
                else
                {
                    filter &= filterDefinitioinBuilder.Eq("_CarModelId", objectModelId);
                }
               
            }
            if(seats.HasValue){
                filter &= filterDefinitioinBuilder.Eq(car => car.Seats, seats.Value);
            }
            if(!string.IsNullOrWhiteSpace(type)){
                filter &= filterDefinitioinBuilder.Eq(car => car.Type, type);
            }if(!string.IsNullOrWhiteSpace(color)){
                filter &= filterDefinitioinBuilder.Eq(car => car.Color, color);
            }
            if(isAvailable.HasValue){
                filter &= filterDefinitioinBuilder.Eq(car => car.IsAvailable, isAvailable.Value);
            }
            filter &= filterDefinitioinBuilder.Gte(car => car.Power, minPower ?? 0);
            filter &= filterDefinitioinBuilder.Lte(car => car.Power, maxPower ?? int.MaxValue);

            filter &= filterDefinitioinBuilder.Gte(car => car.Price_per_day, minPricePerDay ?? 0);
            filter &= filterDefinitioinBuilder.Lte(car => car.Price_per_day, maxPricePerDay ?? int.MaxValue);

            filter &= filterDefinitioinBuilder.Gte(car => car.Curr_mileage, minCurrMileage ?? 0);
            filter &= filterDefinitioinBuilder.Lte(car => car.Curr_mileage, maxCurrMileage ?? int.MaxValue);

            filter &= filterDefinitioinBuilder.Gte(car => car.Production_year, minProductionYear ?? 1900);
            filter &= filterDefinitioinBuilder.Lte(car => car.Production_year, maxProductionYear ?? 2100);
            
            var result = await _carService.GetCarsPerFilterAsync(filter);
            if (result.Any())
            {
                return Ok(result);
            }
            else{
                return NotFound("Cars not found.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving cars:: {ex.Message}");
        }
    }

    [HttpGet("Cars/{id}")]
    public async Task<IActionResult> GetCarByIdAsync(string id)
    {
        try
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest("Invalid ObjectId format.");
            }
            var car = await _carService.GetCarByIdAsync(objectId);
            if (car != null)
            {
                return Ok(car);
            }
            else
            {
                return NotFound($"Car with ID {id} not found.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving the car: {ex.Message}");
        }
    }
}
