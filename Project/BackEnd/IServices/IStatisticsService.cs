using Microsoft.AspNetCore.Mvc;

public interface IStatisticsService
{
    Task<IActionResult> TopNCars(int n);
    Task<IActionResult> TopNClientsPerMileage(int n);
    Task<IActionResult> FavCarPerClient();

}