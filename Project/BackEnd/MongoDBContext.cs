using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Bson;

public class MongoDbContext
{
    private readonly ILogger<MongoDbContext> _logger;
    private readonly IMongoDatabase _database;

    public MongoDbContext(ILogger<MongoDbContext> logger)
    {
        _logger = logger;

        string connectionString = "mongodb://localhost:27017";
        string databaseName = "CarRental";

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }
}
