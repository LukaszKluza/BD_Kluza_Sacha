using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Client
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public ObjectId _id { get; set; }


    [BsonElement("first_name")]
    public string? First_Name { get; set; }

    [BsonElement("last_name")]
    public string? Last_Name { get; set; }

    [BsonElement("phone_number")]
    public string? Phone_Number { get; set; }

    [BsonElement("gender")]
    public string? Gender { get; set; }

    [BsonElement("birthday")]
    [BsonDateTimeOptions(DateOnly = true)]
    public DateTime? Birth_Day { get; set; }

    [BsonElement("pesel")]
    public string? Pesel { get; set; }

    [BsonElement("email")]
    public string? Email { get; set; }

    [BsonElement("address")]
    public string? Address { get; set; }

    [BsonElement("city")]
    public string? City { get; set; }

    [BsonElement("country")]
    public string? Country { get; set; }

    [BsonElement("customer_since")]
    [BsonDateTimeOptions(DateOnly = true)]
    public DateTime? Customer_Since { get; set; }

    [BsonElement("total_rental_days")]
    public int? Total_Rental_Days { get; set; }

    [BsonElement("password_hash")]
    public string Password_Hash { get; set; }
}
