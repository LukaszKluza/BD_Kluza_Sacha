using MongoDB.Bson;
using MongoDB.Driver;

public interface ICarService
{
    Task CreateCarAsync(Car car);
    Task<bool> UpdateCarAsync(ObjectId id, Car car);
    Task<bool> DeleteCarAsync(ObjectId id);
    Task<IEnumerable<Car>> GetCarsPerFilterAsync(FilterDefinition<Car> filter);
    Task<Car> GetCarByIdAsync(ObjectId id);
	Task<bool> UpdateCarAvailabilityByIdAsync(ObjectId id, bool availability);
    Task<bool> UpdateCurrentMileageAsync(ObjectId id, int mileage);
}
