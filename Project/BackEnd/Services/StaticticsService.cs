using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

public class StatisticsService : IStatisticsService
{
    private readonly IMongoCollection<Car> _carCollection;
    private readonly IMongoCollection<Client> _clientCollection;

    private readonly ILogger<StatisticsService> _logger;

    public StatisticsService(IMongoCollection<Car> carCollection, IMongoCollection<Client> clientCollection, ILogger<StatisticsService> logger)
    {
        _carCollection = carCollection;
        _clientCollection = clientCollection;
        _logger = logger;
    }

    public async Task<JsonDocument> TopNCars(int n)
    {
        try
        {
            var pipeline = _carCollection.Aggregate()
                .Lookup("Rentals", "_id", "rental_car.carId", "rentalCar")
                .Unwind("rentalCar")
                .Group(new BsonDocument
                {
                    { "_id", "$_id" },
                    { "count", new BsonDocument("$sum", 1) },
                    { "car", new BsonDocument("$first", "$$ROOT") }
                })
                .Sort(new BsonDocument("count", -1))
                .Project(new BsonDocument
                {
                    { "_id", "$car._id" },
                    { "_carModelId", "$car._carModelId" },
                    { "make", "$car.rentalCar.rental_car.make" },
                    { "model", "$car.rentalCar.rental_car.model" },
                    { "seats", "$car.seats" },
                    { "type", "$car.type" },
                    { "color", "$car.color" },
                    { "power", "$car.power" },
                    { "curr_mileage", "$car.curr_mileage" },
                    { "price_per_day", "$car.price_per_day" },
                    { "isAvailable", "$car.isAvailable" },
                    { "production_year", "$car.production_year" },
                    { "count", "$count" }
                })
                .Limit(n);

            var result = await pipeline.ToListAsync();
            var json = result.ToJson();
            var jsonDocument = JsonDocument.Parse(json);
            return jsonDocument;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving top cars: {ex.Message}");
            throw;
        }
    }
    public async Task<JsonDocument> TopNClientsPerMileage(int n)
    {
        try
        {
            var pipeline = _clientCollection.Aggregate()
                .Group(new BsonDocument
                {
                    { "_id", "$_id" },
                    { "sum", new BsonDocument("$sum", "$total_rental_days") },
                    { "customer", new BsonDocument("$first", "$$ROOT") }
                })
                .Sort(new BsonDocument("count", -1))
                .Project(new BsonDocument
                {
                    { "customer", 1 },
                })
                .Limit(n);

            var result = await pipeline.ToListAsync();
            var json = result.ToJson();
            var jsonDocument = JsonDocument.Parse(json);
            return jsonDocument;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving top cars: {ex.Message}");
            throw;
        }
    }
    public async Task<JsonDocument> FavCarPerClient()
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
                    { "_id", 0 }, // 0 to wyłączenie pola _id z wyników
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
            var json = result.ToJson();
            var jsonDocument = JsonDocument.Parse(json);
            return jsonDocument;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving top cars: {ex.Message}");
            throw;
        }
    }
}
