using MongoDB.Driver;

public interface IClientService
{
    Task CreateClientAsync(Client client);
    Task<bool> UpdateClientAsync(int id, Client client);
    Task<bool> DeleteClientAsync(int id);
    Task<IEnumerable<Client>> GetClientsPerFilterAsync(FilterDefinition<Client> filter);
}
