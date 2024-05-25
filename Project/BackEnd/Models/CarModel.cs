using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class CarModel
{
    [BsonId]
    [BsonElement("_id")]
    public int _id { get; set; }

    [BsonElement("mark")]
    public string Mark { get; set; }

    [BsonElement("model")]
    public string Model { get; set; }

    [BsonElement("type")]
    public string[] Type { get; set; }
}
