using System.ComponentModel.DataAnnotations;

namespace TrendLens.Core.DTOs;

/// <summary>
/// DTO representing monthly sales trend data.
/// </summary>
public sealed class MonthlyTrendDto
{
    /// <summary>
    /// Month and year (e.g., "2024-01").
    /// </summary>
    [Required, StringLength(7, MinimumLength = 7)]
    public string Month { get; init; } = string.Empty;

    /// <summary>
    /// Total revenue for the month.
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal Revenue { get; init; }

    /// <summary>
    /// Total number of orders.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int OrderCount { get; init; }

    /// <summary>
    /// Average order value.
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal AverageOrderValue { get; init; }

    /// <summary>
    /// Percentage change from the previous month.
    /// </summary>
    public decimal PercentageChange { get; init; }

    /// <summary>
    /// Growth trend indicator (e.g., "up", "down", "flat").
    /// </summary>
    [StringLength(20)]
    public string Trend { get; init; } = string.Empty;
}

/// <summary>
/// DTO representing product performance data.
/// </summary>
public sealed class ProductPerformanceDto
{
    /// <summary>
    /// Product name.
    /// </summary>
    [Required, StringLength(100)]
    public string ProductName { get; init; } = string.Empty;

    /// <summary>
    /// Product category.
    /// </summary>
    [Required, StringLength(100)]
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Total revenue for the product.
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal TotalRevenue { get; init; }

    /// <summary>
    /// Total quantity sold.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int TotalQuantity { get; init; }

    /// <summary>
    /// Average price per unit.
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal AveragePrice { get; init; }

    /// <summary>
    /// Number of orders for this product.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int OrderCount { get; init; }

    /// <summary>
    /// Market share percentage (0–100).
    /// </summary>
    [Range(0, 100)]
    public decimal MarketShare { get; init; }

    /// <summary>
    /// Growth rate compared to the previous period.
    /// </summary>
    public decimal GrowthRate { get; init; }
}

/// <summary>
/// DTO representing sales forecast data.
/// </summary>
public sealed class SalesForecastDto
{
    /// <summary>
    /// Forecast date.
    /// </summary>
    [Required, DataType(DataType.Date)]
    public DateTime Date { get; init; }

    /// <summary>
    /// Predicted revenue for the forecast date.
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal PredictedRevenue { get; init; }

    /// <summary>
    /// Confidence interval percentage (0–100).
    /// </summary>
    [Range(0, 100)]
    public decimal ConfidenceInterval { get; init; }

    /// <summary>
    /// Lower bound of the forecast.
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal LowerBound { get; init; }

    /// <summary>
    /// Upper bound of the forecast.
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal UpperBound { get; init; }

    /// <summary>
    /// Type of forecast (e.g., "statistical", "ml_service", "emergency").
    /// </summary>
    [Required, StringLength(50)]
    public string ForecastType { get; init; } = string.Empty;

    /// <summary>
    /// Additional metadata for the forecast.
    /// </summary>
    public Dictionary<string, object> Metadata { get; init; } = new();
}
