using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Car
{
    [BsonId]
    [BsonElement("_id")]
    public int _id { get; set; } 

    [BsonElement("_carModelId")]
    public int _CarModelId { get; set; }

    [BsonElement("seats")]
    public int Seats { get; set; }

    [BsonElement("type")]
    public string Type { get; set; }

    [BsonElement("color")]
    public string Color { get; set; }

    [BsonElement("power")]
    public int Power { get; set; }

    [BsonElement("curr_mileage")]
    public int Curr_mileage { get; set; }

    [BsonElement("price_per_day")]
    public double Price_per_day { get; set; }

    [BsonElement("isAvailable")]
    public bool IsAvailable { get; set; }

    [BsonElement("production_year")]
    public int Production_year { get; set; }
}
