using MongoDB.Bson;
using MongoDB.Driver;

public interface IClientService
{
    Task CreateClientAsync(Client client);
    Task<bool> UpdateClientAsync(ObjectId id, Client client);
    Task<bool> UpdateRentalDaysAsync(ObjectId id, int rental_days);
    Task<bool> DeleteClientAsync(ObjectId id);

    Task<Client> GetUserByEmailAsync(string email);
    Task<IEnumerable<Client>> GetClientsPerFilterAsync(FilterDefinition<Client> filter);
}
