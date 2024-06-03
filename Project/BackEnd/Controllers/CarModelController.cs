using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;


[Route("api/[controller]")]
[ApiController]

public class CarModelController : ControllerBase
{
    private readonly ICarsModelsService _carsModelsService;

    public CarModelController(ICarsModelsService carsModelsService)
    {
        _carsModelsService = carsModelsService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCarModel([FromBody] CarModel carModel)
    {
        try
        {
            await _carsModelsService.CreateCarModelAsync(carModel);
            return Ok("Car model created successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the car model: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCarModel(string id, [FromBody] CarModel carModel)
    {
        try
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest("Invalid ObjectId format.");
            }
            var success = await _carsModelsService.UpdateCarModelAsync(objectId, carModel);
            if (success)
            {
                return Ok($"Car model with ID '{id}' updated successfully.");
            }
            else
            {
                return NotFound("Car model not found.");
            }
           
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the car model: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCarModel(string id)
    {
        try
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest("Invalid ObjectId format.");
            }
            var success = await _carsModelsService.DeleteCarModelAsync(objectId);
            if (success)
            {
                return Ok("Car model deleted successfully.");
            }
            else
            {
                return NotFound("Car model not found.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the car model: {ex.Message}");
        }
    }

    [HttpGet("Models")]
    public async Task<IActionResult> GetCarsModelsPerFilterAsync(string? mark = null, string? model = null)
    {
        try
        {
            var filterDefinitioinBuilder = Builders<CarModel>.Filter;
            var filter = Builders<CarModel>.Filter.Empty;

            if(!string.IsNullOrWhiteSpace(mark)){
                filter &= filterDefinitioinBuilder.Eq(carModel => carModel.Mark, mark);
            }if(!string.IsNullOrWhiteSpace(model)){
                filter &= filterDefinitioinBuilder.Eq(carModel => carModel.Model, model);
            }
            
            var result = await _carsModelsService.GetCarsModelsPerFilterAsync(filter);
            if (result.Any())
            {
                return Ok(result);
            }
            else{
                return NotFound("Cars models not found.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving cars models: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCarByIdAsync(string id)
    {
        try
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest("Invalid ObjectId format.");
            }
            var carModel = await _carsModelsService.GetCarModelByIdAsync(objectId);
            if (carModel != null)
            {
                Console.WriteLine(carModel._id);
                return Ok(carModel);
            }
            else
            {
                return NotFound($"Car model with ID {id} not found.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving the car model: {ex.Message}");
        }
    }
}