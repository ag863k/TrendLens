using Microsoft.AspNetCore.Mvc;
using TrendLens.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace TrendLens.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MachineLearningController : ControllerBase
{
    private readonly IMachineLearningService _mlService;
    private readonly ILogger<MachineLearningController> _logger;

    public MachineLearningController(IMachineLearningService mlService, ILogger<MachineLearningController> logger)
    {
        _mlService = mlService;
        _logger = logger;
    }

    [HttpGet("forecast")]
    public async Task<IActionResult> GetSalesForecast([FromQuery][Range(1, 365)] int days = 30)
    {
        try
        {
            var forecast = await _mlService.GetSalesForecastAsync(days);
            return Ok(forecast);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sales forecast for {Days} days", days);
            return StatusCode(500, "Internal server error occurred while generating forecast");
        }
    }

    [HttpGet("customer-lifetime-value/{customerName}")]
    public async Task<IActionResult> GetCustomerLifetimeValue(string customerName)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            return BadRequest("Customer name is required");

        try
        {
            var clv = await _mlService.PredictCustomerLifetimeValueAsync(customerName);
            return Ok(new { CustomerName = customerName, PredictedLifetimeValue = clv });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error predicting customer lifetime value for {CustomerName}", customerName);
            return StatusCode(500, "Internal server error occurred while predicting customer lifetime value");
        }
    }

    [HttpPost("train")]
    public async Task<IActionResult> TrainModel()
    {
        try
        {
            await _mlService.TrainModelAsync();
            _logger.LogInformation("Machine learning model training completed successfully");
            return Ok(new { Message = "Model training completed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error training machine learning model");
            return StatusCode(500, "Internal server error occurred while training the model");
        }
    }
}
