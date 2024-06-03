using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Linq.Expressions;


[Route("api/[controller]")]
[ApiController]
public class RentalController : ControllerBase
{
    private readonly IRentalService _rentalService;

    public RentalController(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    [HttpPost("NewRental")]
    public async Task<IActionResult> CreateNewRental([FromBody] Rental rental)
    {
        if(!CheckRental(rental)){
            return StatusCode(401, "Some value are invalid");
        }
        try
        {
            await _rentalService.CreateRentalAsync(rental);
            return Ok("Rental created successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the car: {ex.Message}");
        }
    }

    [HttpPost("FinishRental/{id}")]
    public async Task<IActionResult> UpdateRental(int id, [FromBody] Rental rental)
    {
        if(!CheckRental(rental)){
            return StatusCode(401, "Some value are invalid");
        }
        try
        {
            Rental finished_rental = await _rentalService.FinishRentalAsync(id, rental);
            return Ok(finished_rental);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while finishing the car: {ex.Message}");
        }
    }

    private bool CheckRental(Rental rental)
    {
        Rental_Details rental_Details = rental.Rental_Details;

        if (rental_Details.Start_Date >= rental_Details.Expected_End_Date)
        {
            return false;
        }
        if (rental_Details.Days <= 0)
        {
            return false;
        }
        if (rental_Details.Discount >= 1 || rental_Details.Discount < 0 || rental_Details.Price < 0 || rental_Details.Extra_Amount < 0
        || rental_Details.Extra_Fuel_Amount < 0 || rental_Details.Extra_Days_Amount < 0 || rental_Details.Extra_Insurance_Amount < 0
        || rental_Details.Final_Amount < 0 || rental_Details.Extra_Mileage_Amount < 0 || rental_Details.Mileage < 0 || rental_Details.Extra_Fuel <0)
        {
            return false;
        }
        return true;
    }
    [HttpGet("Rentals")]
    public async Task<IActionResult> GetRentalsPerFilterAsync(string? clientId = null, int? carId = null, string? make = null, string? model = null, 
    double? minPricePerDay = null, double? maxPricePerDay = null, DateTime? minStartDate = null, DateTime? maxStartDate = null, DateTime? minExpectedEndDate = null, 
    DateTime? maxExpectedEndDate = null, DateTime? minEndDate = null, DateTime? maxEndDate = null, string? rentalStatus = null, string? insuranceType = null,
    double? minExtraInsuranceAmount = null, double? maxExtraInsuranceAmount = null, int? minDays = null, int? maxDays = null, double? minExtraDaysAmount = null, 
    double? maxExtraDaysAmount = null, int? minMileage = null, int? maxMileage = null, double? minExtraMileageAmount = null, double? maxExtraMileageAmount = null,
    int? minExtraFuel = null, int? maxExtraFuel = null, double? minExtraFuelAmount = null, double? maxExtraFuelAmount = null, double? minPrice = null, double? maxPrice = null,
    double? minDiscount = null, double? maxDiscount = null, double? minExtraAmount = null, double? maxExtraAmount = null, double? minFinalAmount = null, double? maxFinalAmount = null)
    {
        try
        {
            var filterDefinitioinBuilder = Builders<Rental>.Filter;
            var filter = Builders<Rental>.Filter.Empty;

            if(!string.IsNullOrWhiteSpace(clientId)){
                filter &= filterDefinitioinBuilder.Eq(rental => rental.Customer.ClientId, clientId);
            }
            if(carId.HasValue){
                filter &= filterDefinitioinBuilder.Eq(rental => rental.Rental_Car.carId, carId.Value);
            }
            if(!string.IsNullOrWhiteSpace(make)){
                filter &= filterDefinitioinBuilder.Eq(rental => rental.Rental_Car.Make, make);
            }if(!string.IsNullOrWhiteSpace(model)){
                filter &= filterDefinitioinBuilder.Eq(rental => rental.Rental_Car.Model, model);
            }
            if(!string.IsNullOrWhiteSpace(rentalStatus)){
                filter &= filterDefinitioinBuilder.Eq(rental => rental.Rental_Details.Rental_Status, rentalStatus);
            }
            if(!string.IsNullOrWhiteSpace(insuranceType)){
                filter &= filterDefinitioinBuilder.Eq(rental => rental.Rental_Details.Insurance_Type, insuranceType);
            }
            
            filter &= AddRangeFilter(filter, filterDefinitioinBuilder, rental => rental.Rental_Car.Price_Per_Day, minPricePerDay, maxPricePerDay);

            filter &= AddDateRangeFilter(filter, filterDefinitioinBuilder, rental => rental.Rental_Details.Start_Date, minStartDate, maxStartDate);
            filter &= AddDateRangeFilter(filter, filterDefinitioinBuilder, rental => rental.Rental_Details.Expected_End_Date, minExpectedEndDate, maxExpectedEndDate);
            filter &= AddDateRangeFilter(filter, filterDefinitioinBuilder, rental => rental.Rental_Details.End_Date, minEndDate, maxEndDate);

            filter &= AddRangeFilter(filter, filterDefinitioinBuilder, rental => rental.Rental_Details.Extra_Mileage_Amount, minExtraInsuranceAmount, maxExtraInsuranceAmount);
            filter &= AddRangeFilter(filter, filterDefinitioinBuilder, rental => rental.Rental_Details.Days, minDays, maxDays);
            filter &= AddRangeFilter(filter, filterDefinitioinBuilder, rental => rental.Rental_Details.Extra_Days_Amount, minExtraDaysAmount, maxExtraDaysAmount);
            filter &= AddRangeFilter(filter, filterDefinitioinBuilder, rental => rental.Rental_Details.Mileage, minMileage, maxMileage);
            filter &= AddRangeFilter(filter, filterDefinitioinBuilder, rental => rental.Rental_Details.Extra_Mileage_Amount, minExtraMileageAmount, maxExtraMileageAmount);
            filter &= AddRangeFilter(filter, filterDefinitioinBuilder, rental => rental.Rental_Details.Extra_Fuel, minExtraFuel, maxExtraFuel);
            filter &= AddRangeFilter(filter, filterDefinitioinBuilder, rental => rental.Rental_Details.Extra_Fuel_Amount, minExtraFuelAmount, maxExtraFuelAmount);
            filter &= AddRangeFilter(filter, filterDefinitioinBuilder, rental => rental.Rental_Details.Price, minPrice, maxPrice);

            filter &= filterDefinitioinBuilder.Gte(rental => rental.Rental_Details.Discount, minDiscount ?? 0);
            filter &= filterDefinitioinBuilder.Lte(rental => rental.Rental_Details.Discount, maxDiscount ?? 1);

            filter &= AddRangeFilter(filter, filterDefinitioinBuilder, rental => rental.Rental_Details.Extra_Amount, minExtraAmount, maxExtraAmount);
            filter &= AddRangeFilter(filter, filterDefinitioinBuilder, rental => rental.Rental_Details.Final_Amount, minFinalAmount, maxFinalAmount);

            var result = await _rentalService.GetRentalsPerFilterAsync(filter);
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

     private static FilterDefinition<T> AddRangeFilter<T>(
        FilterDefinition<T> filter,
        FilterDefinitionBuilder<T> filterBuilder,
        Expression<Func<T, double?>> field,
        double? minValue,
        double? maxValue)
        {
            filter &= filterBuilder.Gte(field, minValue ?? 0);
            filter &= filterBuilder.Lte(field, maxValue ?? double.MaxValue);
            return filter;
        }

    private static FilterDefinition<T> AddRangeFilter<T>(
        FilterDefinition<T> filter,
        FilterDefinitionBuilder<T> filterBuilder,
        Expression<Func<T, int?>> field,
        int? minValue,
        int? maxValue)
        {
            filter &= filterBuilder.Gte(field, minValue ?? 0);
            filter &= filterBuilder.Lte(field, maxValue ?? int.MaxValue);
            return filter;
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