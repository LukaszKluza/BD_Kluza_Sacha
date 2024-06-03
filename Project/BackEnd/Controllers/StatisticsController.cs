using Microsoft.AspNetCore.Mvc;


[Route("api/[controller]")]
[ApiController]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;
    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }
        
    [HttpGet("Rentals/{n}")]
    public async Task<IActionResult>  GetTopNCarsAsync(int n){
        try
        {
            var topNcars = await _statisticsService.TopNCars(n);
            return Ok(topNcars);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while finishing the top n car: {ex.Message}");
        }
    }

    [HttpGet("Customers/{n}")]
    public async Task<IActionResult> GetTopNCustomersPerMileageAsync(int n){
        try
        {
            var topNcustomers = await _statisticsService.TopNClientsPerMileage(n);
            return Ok(topNcustomers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while finishing the top n car: {ex.Message}");
        }
    }
    
    [HttpGet("Customers/Cars")]
    public async Task<IActionResult> GetFavCarPerClient(){
        try
        {
            var topNcustomers = await _statisticsService.FavCarPerClient();
            return Ok(topNcustomers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while finishing the top n car: {ex.Message}");
        }
    }

}
        