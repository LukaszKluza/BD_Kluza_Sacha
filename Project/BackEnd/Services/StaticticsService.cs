using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

public class StatisticsService : IStatisticsService
{
    private readonly IMongoCollection<Car> _carCollection;
    private readonly IMongoCollection<Client> _clientCollection;
    private readonly IMongoCollection<CarModel> _carModelCollection;
    private readonly IMongoCollection<Rental> _rentalCollection;
    private readonly ILogger<StatisticsService> _logger;

    public StatisticsService(IMongoCollection<Car> carCollection, IMongoCollection<Client> clientCollection, 
    IMongoCollection<CarModel> carModelCollection, IMongoCollection<Rental> rentalCollection, ILogger<StatisticsService> logger)
    {
        _carCollection = carCollection;
        _clientCollection = clientCollection;
        _carModelCollection = carModelCollection;
        _rentalCollection = rentalCollection;
        _logger = logger;
    }

    public async Task<IActionResult> TopNCars(int n)
    {
        try
        {
            var pipeline = _rentalCollection.Aggregate()
                .Lookup("Cars", "rental_car.carId", "_id", "cars")
                .Unwind("cars")
                .Group(new BsonDocument
                {
                    { "_id", "$cars._carModelId" },
                    { "count", new BsonDocument("$sum", 1) },
                    { "model", new BsonDocument("$first", "$rental_car.model") },
                    { "make", new BsonDocument("$first", "$rental_car.make") },
                })
                .Sort(new BsonDocument("count", -1))
                .Limit(n);

            var result = await pipeline.ToListAsync();

            var formattedResult = result.Select(doc => doc.ToDictionary(
                element => element.Name,
                element => BsonTypeMapper.MapToDotNetValue(element.Value)
            )).ToList();

            return new JsonResult(formattedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving top cars: {ex.Message}");
            throw;
        }
    }


    public async Task<IActionResult> TopNClientsPerMileage(int n)
    {
        try
        {
            var pipeline = _rentalCollection.Aggregate()
                .Group(new BsonDocument
                {
                    { "_id", "$customer.clientId" },
                    { "sum", new BsonDocument("$sum", "$rental_details.mileage") },
                    { "customer", new BsonDocument("$first", "$customer") }
                })
                .Sort(new BsonDocument("sum", -1))
                .Project(new BsonDocument
                {
                    { "_id", 0 }, 
                    { "customer", 1 },
                    { "sum", 1 }
                })
                .Limit(n);

            var result = await pipeline.ToListAsync();

            var formattedResult = result.Select(doc => doc.ToDictionary(
                element => element.Name,
                element => BsonTypeMapper.MapToDotNetValue(element.Value)
            )).ToList();

            return new JsonResult(formattedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving top clients per mileage: {ex.Message}");
            throw;
        }
    }
    public async Task<IActionResult> FavCarPerClient()
    {
        try
        {
            var pipeline = _clientCollection.Aggregate()
                .Lookup("Rentals", "_id", "customer.clientId", "rental")
                .Unwind("rental")
                .Group(new BsonDocument
                {
                    { "_id", new BsonDocument{ {"clients_id" ,"$_id"}, {"car_id" ,"$rental.rental_car.carId"}}},
                    { "sum", new BsonDocument("$sum", 1) },
                })
                .Group(new BsonDocument
                {
                    { "_id", "$_id.clients_id" },
                    { "maxSum", new BsonDocument("$max", "$sum") },
                    { "cars", new BsonDocument("$push", new BsonDocument
                        {
                            { "car_id", "$_id.car_id" },
                            { "sum", "$sum" }
                        })
                    }
                })
                .Project(new BsonDocument
                {
                    { "_id", 0 },
                    { "customer", "$_id" },
                    { "filteredCars", new BsonDocument
                        {
                            { "$filter", new BsonDocument
                                {
                                    { "input", "$cars" },
                                    { "as", "car" },
                                    { "cond", new BsonDocument("$eq", new BsonArray { "$$car.sum", "$maxSum" }) }
                                }
                            }
                        }
                    }
                });

            var result = await pipeline.ToListAsync();
            var formattedResult = result.Select(doc => doc.ToDictionary(
                element => element.Name,
                element => BsonTypeMapper.MapToDotNetValue(element.Value)
            )).ToList();

            return new JsonResult(formattedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving favorite car per customer: {ex.Message}");
            throw;
        }
    }
}
