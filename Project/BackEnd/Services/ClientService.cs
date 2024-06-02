using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System.Threading.Tasks;


public class ClientService : IClientService
{
    private readonly IMongoCollection<Client> _clientCollection;
    private readonly ILogger<ClientService> _logger;

    public ClientService(IMongoCollection<Client> clientCollection, ILogger<ClientService> logger)
    {
        _clientCollection = clientCollection;
        _logger = logger;
    }

    public async Task CreateClientAsync(Client client)
    {
        try
        {
            _logger.LogInformation("Attempting to create client: {@Client}", client);
            if (_clientCollection == null)
            {
                _logger.LogError("Clients collection is null");
                return;
            }
            await _clientCollection.InsertOneAsync(client);
            _logger.LogInformation("Client created successfully: {@Client}", client);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating client");
            throw;
        }
    }

    public Task<bool> DeleteClientAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Client>> GetClientsPerFilterAsync(FilterDefinition<Client> filter)
    {
        throw new NotImplementedException();
    }

    public async Task<Client> GetUserByEmailAsync(string email)
    {
        try 
        {
            var result =  await _clientCollection.Find(client => client.Email == email).FirstOrDefaultAsync();
            return result;
        }
        catch(Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving client: {ex.Message}");
            throw;
        }
    }

    public Task<bool> UpdateClientAsync(int id, Client client)
    {
        throw new NotImplementedException();
    }
}
