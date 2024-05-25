using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Threading.Tasks;

public class CarsModelsService : ICarsModelsService
{
    private readonly IMongoCollection<CarModel> _carModelCollection;
    private readonly ILogger<CarsModelsService> _logger;

    public CarsModelsService(IMongoCollection<CarModel> carModelCollection, ILogger<CarsModelsService> logger)
    {
        _carModelCollection = carModelCollection;
        _logger = logger;
    }

    public async Task CreateCarModelAsync(CarModel carModel)
    {
        try
        {
            _logger.LogInformation("Attempting to create car model: {@CarModel}", carModel);
            if(_carModelCollection == null)
            {
                _logger.LogError("Cars models collection is null");
                return;
            }
            await _carModelCollection.InsertOneAsync(carModel);
            _logger.LogInformation("Car model created successfully: {@CarModel}", carModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while creating the car");
            throw;
        }
    }

    public async Task<bool> UpdateCarModelAsync(int id, CarModel carModel)
    {
        try
        {
            var filter = Builders<CarModel>.Filter.Eq(carModel => carModel._id, id);
            
            var originalCarModel = await _carModelCollection.Find(filter).FirstOrDefaultAsync();

            if (originalCarModel == null)
            {
                _logger.LogWarning($"Car model with ID '{id}' not found.");
                return false;
            }

            carModel._id = originalCarModel._id;

            var result = await _carModelCollection.ReplaceOneAsync(filter, carModel);

            if (result.ModifiedCount > 0)
            {
                _logger.LogInformation($"Car model with ID '{id}' updated successfully.");
                return true;
            }
            else
            {
                _logger.LogWarning($"Car model with ID '{id}' not found.");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while updating the car model: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteCarModelAsync(int id)
    {
       try
       {
            var result = await _carModelCollection.DeleteOneAsync(carModel => carModel._id == id);
            if (result.DeletedCount > 0)
            {
                _logger.LogInformation($"Car model with ID '{id}' deleted successfully.");
                return true;
            }
            else
            {
                _logger.LogWarning($"car with ID '{id}' not found.");
                return false;
            }
       }
       catch (Exception ex)
       {
            _logger.LogError($"An error occured while deleting the car model: {ex.Message}");
            throw;
       }
    }   

    public async Task<IEnumerable<CarModel>> GetCarsModelsPerFilterAsync(FilterDefinition<CarModel> filter)
    {
        try
        {
            var result = await _carModelCollection.Find(filter).ToListAsync();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving cars models: {ex.Message}");
            throw;
        }
    }

    public async Task<CarModel> GetCarModelByIdAsync(int id)
    {
        var filter = Builders<CarModel>.Filter.Eq(carModel => carModel._id, id);
        var carModel = await _carModelCollection.Find(filter).FirstOrDefaultAsync();
        return carModel;
    }
}