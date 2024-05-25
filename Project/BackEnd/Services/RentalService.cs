using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;


public class RentalService : IRentalService
{
    private readonly IMongoCollection<Rental> _rentalCollection;
    private readonly ILogger<RentalService> _logger;

    public RentalService(IMongoCollection<Rental> rentalCollection, ILogger<RentalService> logger)
    {
        _rentalCollection = rentalCollection;
        _logger = logger;
    }
    public async Task CreateRentalAsync(Rental rental)
    {
        try
        {
            _logger.LogInformation("Attempting to create rental model: {@Rental}", rental);
            if(_rentalCollection == null)
            {
                _logger.LogError("Rentals models collection is null");
                return;
            }
            await _rentalCollection.InsertOneAsync(rental);
            _logger.LogInformation("Rental model created successfully: {@Rental}", rental);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while creating the rental");
            throw;
        }
    }

    public async Task<Rental> FinishRentalAsync(int id, Rental rental)
    {
        var filter = Builders<Rental>.Filter.Eq(rental => rental._id, id);
            
        var originalRental = await _rentalCollection.Find(filter).FirstOrDefaultAsync();

        if (originalRental == null || rental == null)
        {
           _logger.LogWarning($"Rental with ID '{id}' not found.");
            throw new KeyNotFoundException($"Rental does not exist.");
        }
        Rental_Details orginal_rental_Details = originalRental.Rental_Details;

        Rental_Details rental_Details = rental.Rental_Details;
        Rental_Car rental_Car = rental.Rental_Car;

        rental_Details.End_Date = DateTime.UtcNow;
        rental_Details.Days = (int)Math.Ceiling((rental_Details.End_Date.Value - rental_Details.Start_Date).TotalDays);
        rental_Details.Rental_Status = "finished";

        if(rental_Details.Days > orginal_rental_Details.Days){
            rental_Details.Extra_Days_Amount = (int)((rental_Details.Days - orginal_rental_Details.Days) * 0.5 * rental_Car.Price_Per_Day);
            rental_Details.Extra_Amount += rental_Details.Extra_Days_Amount;
        }
        if(rental_Details.Mileage > orginal_rental_Details.Mileage){
            rental_Details.Extra_Mileage_Amount = (int)((orginal_rental_Details.Mileage - rental_Details.Mileage) * 0.005 * rental_Car.Price_Per_Day);
            rental_Details.Extra_Mileage_Amount += rental_Details.Extra_Mileage_Amount;
        }
        if(rental_Details.Extra_Fuel != null){
            rental_Details.Extra_Fuel_Amount = rental_Details.Extra_Fuel.Value * 5;
            rental_Details.Extra_Amount += rental_Details.Extra_Fuel_Amount;
        }
        rental_Details.Final_Amount = (int)(rental_Details.Price * (1-orginal_rental_Details.Discount) + rental_Details.Extra_Amount);
        var result = await _rentalCollection.ReplaceOneAsync(filter, rental);
        return rental;
    }

    public async Task<IEnumerable<Rental>> GetRentalsPerFilterAsync(FilterDefinition<Rental> filter)
    {
        try
        {
            var result = await _rentalCollection.Find(filter).ToListAsync();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving rentals: {ex.Message}");
            throw;
        }
    }
}