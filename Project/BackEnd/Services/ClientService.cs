using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System.Threading.Tasks;


public class ClientService : IClientService
{
    private readonly IMongoCollection<Car> _carCollection;
    private readonly ILogger<ClientService> _logger;

    public ClientService(IMongoCollection<Car> carCollection, ILogger<ClientService> logger)
    {
        _carCollection = carCollection;
        _logger = logger;
    }

    public Task CreateClientAsync(Client client)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteClientAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Client>> GetClientsPerFilterAsync(FilterDefinition<Client> filter)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateClientAsync(int id, Client client)
    {
        throw new NotImplementedException();
    }
}
