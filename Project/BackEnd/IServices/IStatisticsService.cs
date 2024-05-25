using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

public interface IStatisticsService
{
    Task<JsonDocument> TopNCars(int n);
    Task<JsonDocument> TopNClientsPerMileage(int n);
    Task<JsonDocument> FavCarPerClient();

}