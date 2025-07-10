using System.ComponentModel.DataAnnotations;

namespace TrendLens.Core.DTOs;

/// <summary>
/// DTO for monthly sales trend data
/// </summary>
public sealed class MonthlyTrendDto
{
    /// <summary>
    /// Month and year (e.g., "2024-01")
    /// </summary>
    [Required]
    public string Month { get; init; } = string.Empty;

    /// <summary>
    /// Total revenue for the month
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal Revenue { get; init; }

    /// <summary>
    /// Total number of orders
    /// </summary>
    [Range(0, int.MaxValue)]
    public int OrderCount { get; init; }

    /// <summary>
    /// Average order value
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal AverageOrderValue { get; init; }

    /// <summary>
    /// Percentage change from previous month
    /// </summary>
    public decimal PercentageChange { get; init; }

    /// <summary>
    /// Growth trend indicator
    /// </summary>
    public string Trend { get; init; } = string.Empty;
}

/// <summary>
/// DTO for product performance data
/// </summary>
public sealed class ProductPerformanceDto
{
    /// <summary>
    /// Product name
    /// </summary>
    [Required]
    public string ProductName { get; init; } = string.Empty;

    /// <summary>
    /// Product category
    /// </summary>
    [Required]
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Total revenue for the product
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal TotalRevenue { get; init; }

    /// <summary>
    /// Total quantity sold
    /// </summary>
    [Range(0, int.MaxValue)]
    public int TotalQuantity { get; init; }

    /// <summary>
    /// Average price per unit
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal AveragePrice { get; init; }

    /// <summary>
    /// Number of orders for this product
    /// </summary>
    [Range(0, int.MaxValue)]
    public int OrderCount { get; init; }

    /// <summary>
    /// Market share percentage
    /// </summary>
    [Range(0, 100)]
    public decimal MarketShare { get; init; }

    /// <summary>
    /// Growth rate compared to previous period
    /// </summary>
    public decimal GrowthRate { get; init; }
}

/// <summary>
/// DTO for sales forecast data
/// </summary>
public sealed class SalesForecastDto
{
    /// <summary>
    /// Forecast date
    /// </summary>
    [Required]
    public DateTime Date { get; init; }

    /// <summary>
    /// Predicted revenue for the date
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal PredictedRevenue { get; init; }

    /// <summary>
    /// Confidence interval percentage
    /// </summary>
    [Range(0, 100)]
    public decimal ConfidenceInterval { get; init; }

    /// <summary>
    /// Lower bound of the prediction
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal LowerBound { get; init; }

    /// <summary>
    /// Upper bound of the prediction
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal UpperBound { get; init; }

    /// <summary>
    /// Type of forecast (statistical, ml_service, emergency)
    /// </summary>
    [Required]
    public string ForecastType { get; init; } = string.Empty;

    /// <summary>
    /// Additional metadata about the forecast
    /// </summary>
    public Dictionary<string, object> Metadata { get; init; } = new();
}
