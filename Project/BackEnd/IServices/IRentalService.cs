using MongoDB.Bson;
using MongoDB.Driver;

public interface IRentalService
{
    Task CreateRentalAsync(Rental rental);
    Task<Rental> FinishRentalAsync(ObjectId id, Rental rental);
    Task<IEnumerable<Rental>> GetRentalsPerFilterAsync(FilterDefinition<Rental> filter);
}