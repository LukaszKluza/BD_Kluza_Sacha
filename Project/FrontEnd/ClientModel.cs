using System.Text.Json.Serialization;

public class Client
{
    [JsonPropertyName("first_Name")]
    public string First_Name { get; set; }

    [JsonPropertyName("last_Name")]
    public string Last_Name { get; set; }

    [JsonPropertyName("phone_Number")]
    public string Phone_Number { get; set; }

    [JsonPropertyName("gender")]
    public string Gender { get; set; }

    [JsonPropertyName("birth_Day")]
    public DateTime? Birth_Day { get; set; }

    [JsonPropertyName("pesel")]
    public string Pesel { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("customer_Since")]
    public DateTime? Customer_Since { get; set; }

    [JsonPropertyName("total_Rental_Days")]
    public int Total_Rental_Days { get; set; }

    [JsonPropertyName("password_Hash")]
    public string Password_Hash { get; set; }
}
