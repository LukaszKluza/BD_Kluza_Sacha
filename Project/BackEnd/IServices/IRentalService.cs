using MongoDB.Driver;

public interface IRentalService
{
    Task CreateRentalAsync(Rental rental);
    Task<Rental> FinishRentalAsync(int id, Rental rental);
    Task<IEnumerable<Rental>> GetRentalsPerFilterAsync(FilterDefinition<Rental> filter);
}