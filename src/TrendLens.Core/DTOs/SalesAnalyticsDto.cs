using System.ComponentModel.DataAnnotations;

namespace TrendLens.Core.DTOs;

/// <summary>
/// Data transfer object for sales analytics information
/// </summary>
public sealed record SalesAnalyticsDto
{
    /// <summary>
    /// Total revenue across all sales
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal TotalRevenue { get; init; }

    /// <summary>
    /// Total number of sales transactions
    /// </summary>
    [Range(0, int.MaxValue)]
    public int TotalSales { get; init; }

    /// <summary>
    /// Average order value
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal AverageOrderValue { get; init; }

    /// <summary>
    /// Monthly trend data
    /// </summary>
    public IReadOnlyList<MonthlyTrendDto> MonthlyTrends { get; init; } = Array.Empty<MonthlyTrendDto>();

    /// <summary>
    /// Top performing products
    /// </summary>
    public IReadOnlyList<ProductPerformanceDto> TopProducts { get; init; } = Array.Empty<ProductPerformanceDto>();

    /// <summary>
    /// Period covered by this analytics data
    /// </summary>
    public DatePeriodDto Period { get; init; } = new();

    /// <summary>
    /// Growth rate compared to previous period (percentage)
    /// </summary>
    public decimal? GrowthRate { get; init; }

    /// <summary>
    /// Number of unique customers
    /// </summary>
    [Range(0, int.MaxValue)]
    public int UniqueCustomers { get; init; }

    /// <summary>
    /// Number of unique products sold
    /// </summary>
    [Range(0, int.MaxValue)]
    public int UniqueProducts { get; init; }
}

/// <summary>
/// Date period information
/// </summary>
public sealed record DatePeriodDto
{
    /// <summary>
    /// Start date of the period
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// End date of the period
    /// </summary>
    public DateTime? EndDate { get; init; }

    /// <summary>
    /// Display name for the period
    /// </summary>
    public string DisplayName { get; init; } = "All Time";

    /// <summary>
    /// Number of days in the period
    /// </summary>
    public int? DaysInPeriod => StartDate.HasValue && EndDate.HasValue
        ? (int)(EndDate.Value - StartDate.Value).TotalDays + 1
        : null;
}


