using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;

public class CarService : ICarService
{
    private readonly IMongoCollection<Car> _carCollection;
    private readonly ILogger<CarService> _logger;

    public CarService(IMongoCollection<Car> carCollection, ILogger<CarService> logger)
    {
        _carCollection = carCollection;
        _logger = logger;
    }

    public async Task CreateCarAsync(Car car)
    {
        try
        {
            _logger.LogInformation("Attempting to create car: {@Car}", car);
            if (_carCollection == null)
            {
                _logger.LogError("Car collection is null");
                return;
            }
            await _carCollection.InsertOneAsync(car);
            _logger.LogInformation("Car created successfully: {@Car}", car);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the car");
            throw;
        }
    }

    public async Task<bool> UpdateCarAsync(ObjectId id, Car car)
    {
        try
        {
            var filter = Builders<Car>.Filter.Eq(car => car._id, id);
            
            var originalCar = await _carCollection.Find(filter).FirstOrDefaultAsync();

            if (originalCar == null)
            {
                _logger.LogWarning($"Car with ID '{id}' not found.");
                return false;
            }

            car._id = originalCar._id;

            var result = await _carCollection.ReplaceOneAsync(filter, car);

            if (result.ModifiedCount > 0)
            {
                _logger.LogInformation($"Car with ID '{id}' updated successfully.");
                return true;
            }
            else
            {
                _logger.LogWarning($"Car with ID '{id}' not found.");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while updating the car: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteCarAsync(ObjectId id)
    {
        try
        {
            var filter = Builders<Car>.Filter.Eq(car => car._id, id);
            var result = await _carCollection.DeleteOneAsync(filter);
            if (result.DeletedCount > 0)
            {
                _logger.LogInformation($"Car with ID '{id}' deleted successfully.");
                return true;
            }
            else
            {
                _logger.LogWarning($"Car with ID '{id}' not found.");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while deleting the car: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<Car>> GetCarsPerFilterAsync(FilterDefinition<Car> filter)
    {
        try
        {
            var jsonFilter = filter.Render(BsonSerializer.SerializerRegistry.GetSerializer<Car>(), BsonSerializer.SerializerRegistry);
            _logger.LogInformation($"Generated Filter: {jsonFilter}");
            var result = await _carCollection.Find(filter).ToListAsync();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving cars: {ex.Message}");
            throw;
        }
    }

    public async Task<Car> GetCarByIdAsync(ObjectId id)
    {
        var filter = Builders<Car>.Filter.Eq("_id", id);
        var car = await _carCollection.Find(filter).FirstOrDefaultAsync();
        return car;
    }

    public async Task<bool> UpdateCarAvailabilityByIdAsync(ObjectId id, bool availability)
    {
        try
        {
            var car = await GetCarByIdAsync(id);

            if (car == null)
            {
                _logger.LogError($"Car {id} not exist");
                return false;
            }

            car.IsAvailable = availability;
            var updateResult = await UpdateCarAsync(id, car);

            return updateResult;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while uptating car availability: {ex.Message}");
            return false;
        }
    }
    public async Task<bool> UpdateCurrentMileageAsync(ObjectId id, int mileage)
    {
        try
        {
            var filter = Builders<Car>.Filter.Eq(car => car._id, id);
            var car = await _carCollection.Find(filter).FirstOrDefaultAsync();

            if (car == null)
            {
                _logger.LogError($"Car {id} not exist");
                return false;
            }

            car.Curr_mileage += mileage;
            var result = await UpdateCarAsync(id, car);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while uptating car mileage: {ex.Message}");
            return false;
        }
    }
}
