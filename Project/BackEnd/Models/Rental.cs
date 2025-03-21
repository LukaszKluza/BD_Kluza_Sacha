using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

public class Rental_Car
{
    [BsonElement("carId")]
    public ObjectId carId { get; set; }

    [BsonElement("make")]
    public string Make { get; set; }

    [BsonElement("model")]
    public string Model { get; set; }

    [BsonElement("price_per_day")]
    public double Price_Per_Day { get; set; }
}

public class Customer
{
    [BsonElement("clientId")]
    public ObjectId ClientId { get; set; }

    [BsonElement("first_name")]
    public string First_Name { get; set; }

    [BsonElement("last_name")]
    public string Last_Name { get; set; }
}

public class Rental_Details
{
    [BsonElement("start_date")]
    public DateTime Start_Date { get; set; }

    [BsonElement("expected_end_date")]
    public DateTime Expected_End_Date { get; set; }

    [BsonElement("end_date")]
    public DateTime? End_Date { get; set; }

    [BsonElement("rental_status")]
    public string Rental_Status { get; set; }

    [BsonElement("insurance_type")]
    public string Insurance_Type { get; set; }

    [BsonElement("extra_insurance_amount")]
    public double Extra_Insurance_Amount { get; set; }

    [BsonElement("days")]
    public int Days { get; set; }

    [BsonElement("extra_days_amount")]
    public double Extra_Days_Amount { get; set; }

    [BsonElement("mileage")]
    public int Mileage { get; set; }

    [BsonElement("extra_mileage_amount")]
    public double Extra_Mileage_Amount { get; set; }

    [BsonElement("extra_fuel")]
    public int? Extra_Fuel { get; set; }

    [BsonElement("extra_fuel_amount")]
    public double Extra_Fuel_Amount { get; set; }

    [BsonElement("price")]
    public double Price { get; set; }

    [BsonElement("discount")]
    public double Discount { get; set; }

    [BsonElement("extra_amount")]
    public double Extra_Amount { get; set; }

    [BsonElement("final_amount")]
    public double Final_Amount { get; set; }
}

public class Rental
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public ObjectId _id { get; set; } 

    [BsonElement("rental_car")]
    public Rental_Car Rental_Car { get; set; }

    [BsonElement("customer")]
    public Customer Customer { get; set; }

    [BsonElement("rental_details")]
    public Rental_Details Rental_Details { get; set; }
}
