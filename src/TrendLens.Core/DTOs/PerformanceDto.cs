using System.ComponentModel.DataAnnotations;

namespace TrendLens.Core.DTOs;

/// <summary>
/// DTO for regional sales performance data
/// </summary>
public sealed class RegionalPerformanceDto
{
    /// <summary>
    /// Region name
    /// </summary>
    [Required]
    public string Region { get; init; } = string.Empty;

    /// <summary>
    /// Total revenue for the region
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal TotalRevenue { get; init; }

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
    /// Number of unique customers
    /// </summary>
    [Range(0, int.MaxValue)]
    public int CustomerCount { get; init; }

    /// <summary>
    /// Market share percentage
    /// </summary>
    [Range(0, 100)]
    public decimal MarketShare { get; init; }

    /// <summary>
    /// Growth rate compared to previous period
    /// </summary>
    public decimal GrowthRate { get; init; }

    /// <summary>
    /// Performance ranking
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Rank { get; init; }

    /// <summary>
    /// Top-selling product in the region
    /// </summary>
    public string TopProduct { get; init; } = string.Empty;

    /// <summary>
    /// Additional performance metrics
    /// </summary>
    public Dictionary<string, decimal> Metrics { get; init; } = new();
}

/// <summary>
/// DTO for sales representative performance data
/// </summary>
public sealed class SalesRepPerformanceDto
{
    /// <summary>
    /// Sales representative name
    /// </summary>
    [Required]
    public string SalesRep { get; init; } = string.Empty;

    /// <summary>
    /// Region the sales rep operates in
    /// </summary>
    [Required]
    public string Region { get; init; } = string.Empty;

    /// <summary>
    /// Total revenue generated
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal TotalRevenue { get; init; }

    /// <summary>
    /// Total number of orders closed
    /// </summary>
    [Range(0, int.MaxValue)]
    public int OrderCount { get; init; }

    /// <summary>
    /// Average order value
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal AverageOrderValue { get; init; }

    /// <summary>
    /// Number of unique customers served
    /// </summary>
    [Range(0, int.MaxValue)]
    public int CustomerCount { get; init; }

    /// <summary>
    /// Conversion rate percentage
    /// </summary>
    [Range(0, 100)]
    public decimal ConversionRate { get; init; }

    /// <summary>
    /// Performance target achievement percentage
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal TargetAchievement { get; init; }

    /// <summary>
    /// Performance ranking among all sales reps
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Rank { get; init; }

    /// <summary>
    /// Growth rate compared to previous period
    /// </summary>
    public decimal GrowthRate { get; init; }

    /// <summary>
    /// Commission earned
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal Commission { get; init; }

    /// <summary>
    /// Additional performance metrics
    /// </summary>
    public Dictionary<string, decimal> Metrics { get; init; } = new();
}

/// <summary>
/// DTO for sales trend data
/// </summary>
public sealed class SalesTrendDto
{
    /// <summary>
    /// Period identifier (e.g., "2024-Q1", "2024-01")
    /// </summary>
    [Required]
    public string Period { get; init; } = string.Empty;

    /// <summary>
    /// Period type (daily, weekly, monthly, quarterly, yearly)
    /// </summary>
    [Required]
    public string PeriodType { get; init; } = string.Empty;

    /// <summary>
    /// Start date of the period
    /// </summary>
    [Required]
    public DateTime StartDate { get; init; }

    /// <summary>
    /// End date of the period
    /// </summary>
    [Required]
    public DateTime EndDate { get; init; }

    /// <summary>
    /// Total revenue for the period
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal Revenue { get; init; }

    /// <summary>
    /// Number of orders
    /// </summary>
    [Range(0, int.MaxValue)]
    public int OrderCount { get; init; }

    /// <summary>
    /// Average order value
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal AverageOrderValue { get; init; }

    /// <summary>
    /// Percentage change from previous period
    /// </summary>
    public decimal PercentageChange { get; init; }

    /// <summary>
    /// Trend direction (up, down, stable)
    /// </summary>
    [Required]
    public string TrendDirection { get; init; } = string.Empty;

    /// <summary>
    /// Trend strength (weak, moderate, strong)
    /// </summary>
    [Required]
    public string TrendStrength { get; init; } = string.Empty;

    /// <summary>
    /// Seasonal adjustment factor
    /// </summary>
    public decimal SeasonalFactor { get; init; } = 1.0m;
}
