using MongoDB.Driver;

public interface IClientService
{
    Task CreateClientAsync(Client client);
    Task<bool> UpdateClientAsync(string id, Client client);
    Task<bool> UpdateRentalDaysAsync(string id, int rental_days);
    Task<bool> DeleteClientAsync(string id);
    Task<Client> GetUserByEmailAsync(string email);
    Task<IEnumerable<Client>> GetClientsPerFilterAsync(FilterDefinition<Client> filter);
}
