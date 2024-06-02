using Microsoft.Extensions.Logging;
using MongoDB.Driver;

public class MongoDbContext
{
    private readonly ILogger<MongoDbContext> _logger;
    private readonly IMongoDatabase _database;
    private readonly IMongoClient _client;

    public MongoDbContext(ILogger<MongoDbContext> logger)
    {
        _logger = logger;

        string connectionString = "mongodb://localhost:27017";
        string databaseName = "CarRental";

        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }
    public IMongoClient GetClient()
    {
        return _client;
    }
}
