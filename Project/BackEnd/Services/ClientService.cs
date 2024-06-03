using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;


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

    public async Task<bool> DeleteClientAsync(string id)
    {
        try
        {
            var result = await _clientCollection.DeleteOneAsync(client => client._id == id);
            if (result.DeletedCount > 0)
            {
                _logger.LogInformation($"Client with ID '{id}' deleted successfully.");
                return true;
            }
            else
            {
                _logger.LogWarning($"Client with ID '{id}' not found.");
                return false;
            }
        }
        catch(Exception ex)
        {
            _logger.LogError($"An error occurred while deleting the car: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<Client>> GetClientsPerFilterAsync(FilterDefinition<Client> filter)
    {
         try
        {
            var jsonFilter = filter.Render(BsonSerializer.SerializerRegistry.GetSerializer<Client>(), BsonSerializer.SerializerRegistry);
            _logger.LogInformation($"Generated Filter: {jsonFilter}");
            var result = await _clientCollection.Find(filter).ToListAsync();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving cars: {ex.Message}");
            throw;
        }
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

    public async Task<bool> UpdateClientAsync(string id, Client client)
    {
        try
        {
            var filter = Builders<Client>.Filter.Eq(client => client._id, id);
            
            var originalClient = await _clientCollection.Find(filter).FirstOrDefaultAsync();

            if (originalClient == null)
            {
                _logger.LogWarning($"Client with ID '{id}' not found.");
                return false;
            }

            client._id = originalClient._id;

            var result = await _clientCollection.ReplaceOneAsync(filter, client);

            if (result.ModifiedCount > 0)
            {
                _logger.LogInformation($"Client with ID '{id}' updated successfully.");
                return true;
            }
            else
            {
                _logger.LogWarning($"Client with ID '{id}' not found.");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while updating the client: {ex.Message}");
            throw;
        }
    }
    public async Task<bool> UpdateRentalDaysAsync(string id, int rental_days)
    {
        try
        {
            Client client = await _clientCollection.Find(client => client._id == id).FirstOrDefaultAsync();

            if (client == null)
            {
                _logger.LogError($"Car {id} not exist");
                return false;
            }

            client.Total_Rental_Days += rental_days;
            Console.WriteLine(client.Total_Rental_Days);
            var result = await UpdateClientAsync(id, client);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while uptating client rental days: {ex.Message}");
            return false;
        }
    }
}
