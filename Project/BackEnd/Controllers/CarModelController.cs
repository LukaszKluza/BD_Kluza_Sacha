using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
    public async Task<IActionResult> UpdateCarModel(int id, [FromBody] CarModel carModel)
    {
        try
        {
            var success = await _carsModelsService.UpdateCarModelAsync(id, carModel);
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
    public async Task<IActionResult> DeleteCarModel(int id)
    {
        try
        {
            var success = await _carsModelsService.DeleteCarModelAsync(id);
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
            return StatusCode(500, $"An error occurred while retrieving cars models:: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCarByIdAsync(int id)
    {
        try
        {
            var carModel = await _carsModelsService.GetCarModelByIdAsync(id);
            if (carModel != null)
            {
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