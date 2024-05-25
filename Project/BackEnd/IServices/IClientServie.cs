using MongoDB.Driver;

public interface IClientService
{
    Task CreateClientAsync(Client client);
    Task<bool> UpdateClientAsync(int id, Client client);
    Task<bool> DeleteClientAsync(int id);
    Task<Client> GetUserByEmailAsync(string email);
    Task<IEnumerable<Client>> GetClientsPerFilterAsync(FilterDefinition<Client> filter);
}
