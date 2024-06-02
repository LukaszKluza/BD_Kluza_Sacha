using System.Text.Json;

public interface IStatisticsService
{
    Task<JsonDocument> TopNCars(int n);
    Task<JsonDocument> TopNClientsPerMileage(int n);
    Task<JsonDocument> FavCarPerClient();

}