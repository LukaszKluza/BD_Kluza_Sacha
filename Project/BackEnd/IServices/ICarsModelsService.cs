using MongoDB.Bson;
using MongoDB.Driver;

public interface ICarsModelsService
{
    Task CreateCarModelAsync(CarModel carModel);
    Task<bool> UpdateCarModelAsync(ObjectId id, CarModel carModel);
    Task<bool> DeleteCarModelAsync(ObjectId id);
    Task<IEnumerable<CarModel>> GetCarsModelsPerFilterAsync(FilterDefinition<CarModel> filter);
    Task<CarModel> GetCarModelByIdAsync(ObjectId id);
}