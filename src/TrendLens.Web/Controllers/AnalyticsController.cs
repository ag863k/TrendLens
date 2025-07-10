using Microsoft.AspNetCore.Mvc;
using TrendLens.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace TrendLens.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsServiceV2 _analyticsService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(IAnalyticsServiceV2 analyticsService, ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    /// <summary>
    /// Get comprehensive sales analytics overview
    /// </summary>
    [HttpGet("overview")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetSalesAnalytics(
        [FromQuery] DateTime? startDate, 
        [FromQuery] DateTime? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            return BadRequest("Start date cannot be greater than end date");

        var result = await _analyticsService.GetAdvancedAnalyticsAsync(startDate, endDate);
        
        if (!result.IsSuccess)
            return StatusCode(500, result.Error);
        
        return Ok(result.Data);
    }

    /// <summary>
    /// Get monthly revenue trends
    /// </summary>
    [HttpGet("trends")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetMonthlyTrends([FromQuery][Range(1, 60)] int months = 12)
    {
        try
        {
            var trends = await _analyticsService.GetMonthlyTrendsAsync(months);
            return Ok(trends);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving monthly trends for {Months} months", months);
            return StatusCode(500, "Internal server error occurred while retrieving trends");
        }
    }

    /// <summary>
    /// Get top performing products
    /// </summary>
    [HttpGet("top-products")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetTopProducts([FromQuery][Range(1, 100)] int count = 10)
    {
        try
        {
            var products = await _analyticsService.GetTopProductsAsync(count);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving top {Count} products", count);
            return StatusCode(500, "Internal server error occurred while retrieving top products");
        }
    }

    /// <summary>
    /// Get regional performance analysis
    /// </summary>
    [HttpGet("regional-performance")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetRegionalPerformance()
    {
        var result = await _analyticsService.GetRegionalPerformanceAsync();
        
        if (!result.IsSuccess)
            return StatusCode(500, result.Error);
        
        return Ok(result.Data);
    }

    /// <summary>
    /// Get sales representative performance analysis
    /// </summary>
    [HttpGet("sales-rep-performance")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetSalesRepPerformance()
    {
        var result = await _analyticsService.GetSalesRepPerformanceAsync();
        
        if (!result.IsSuccess)
            return StatusCode(500, result.Error);
        
        return Ok(result.Data);
    }
}
