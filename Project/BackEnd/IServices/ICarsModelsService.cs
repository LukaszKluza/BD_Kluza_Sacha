using MongoDB.Driver;

public interface ICarsModelsService
{
    Task CreateCarModelAsync(CarModel carModel);
    Task<bool> UpdateCarModelAsync(int id, CarModel carModel);
    Task<bool> DeleteCarModelAsync(int id);
    Task<IEnumerable<CarModel>> GetCarsModelsPerFilterAsync(FilterDefinition<CarModel> filter);
    Task<CarModel> GetCarModelByIdAsync(int id);
}