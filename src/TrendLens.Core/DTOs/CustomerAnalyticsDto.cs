using System.ComponentModel.DataAnnotations;

namespace TrendLens.Core.DTOs;

/// <summary>
/// DTO for customer analytics data
/// </summary>
public sealed class CustomerAnalyticsDto
{
    /// <summary>
    /// Total number of unique customers
    /// </summary>
    [Range(0, int.MaxValue)]
    public int TotalCustomers { get; init; }

    /// <summary>
    /// Number of new customers in the period
    /// </summary>
    [Range(0, int.MaxValue)]
    public int NewCustomers { get; init; }

    /// <summary>
    /// Number of returning customers
    /// </summary>
    [Range(0, int.MaxValue)]
    public int ReturningCustomers { get; init; }

    /// <summary>
    /// Customer retention rate percentage
    /// </summary>
    [Range(0, 100)]
    public decimal RetentionRate { get; init; }

    /// <summary>
    /// Average customer lifetime value
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal AverageCustomerLifetimeValue { get; init; }

    /// <summary>
    /// Average order frequency per customer
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal AverageOrderFrequency { get; init; }

    /// <summary>
    /// Average order value per customer
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal AverageOrderValue { get; init; }

    /// <summary>
    /// Customer acquisition cost
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal CustomerAcquisitionCost { get; init; }

    /// <summary>
    /// Churn rate percentage
    /// </summary>
    [Range(0, 100)]
    public decimal ChurnRate { get; init; }

    /// <summary>
    /// Top customers by revenue
    /// </summary>
    public IEnumerable<TopCustomerDto> TopCustomers { get; init; } = Enumerable.Empty<TopCustomerDto>();

    /// <summary>
    /// Customer segmentation data
    /// </summary>
    public CustomerSegmentationDto Segmentation { get; init; } = new();

    /// <summary>
    /// Geographic distribution of customers
    /// </summary>
    public IEnumerable<CustomerGeographyDto> GeographicDistribution { get; init; } = Enumerable.Empty<CustomerGeographyDto>();

    /// <summary>
    /// Additional customer metrics
    /// </summary>
    public Dictionary<string, decimal> AdditionalMetrics { get; init; } = new();
}

/// <summary>
/// DTO for top customer data
/// </summary>
public sealed class TopCustomerDto
{
    /// <summary>
    /// Customer name
    /// </summary>
    [Required]
    public string CustomerName { get; init; } = string.Empty;

    /// <summary>
    /// Total revenue from this customer
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal TotalRevenue { get; init; }

    /// <summary>
    /// Number of orders placed
    /// </summary>
    [Range(0, int.MaxValue)]
    public int OrderCount { get; init; }

    /// <summary>
    /// Average order value
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal AverageOrderValue { get; init; }

    /// <summary>
    /// Last order date
    /// </summary>
    public DateTime? LastOrderDate { get; init; }

    /// <summary>
    /// Customer lifetime value
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal LifetimeValue { get; init; }

    /// <summary>
    /// Customer ranking
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Rank { get; init; }
}

/// <summary>
/// DTO for customer segmentation data
/// </summary>
public sealed class CustomerSegmentationDto
{
    /// <summary>
    /// High-value customers (top 20% by revenue)
    /// </summary>
    [Range(0, int.MaxValue)]
    public int HighValueCustomers { get; init; }

    /// <summary>
    /// Medium-value customers (middle 60% by revenue)
    /// </summary>
    [Range(0, int.MaxValue)]
    public int MediumValueCustomers { get; init; }

    /// <summary>
    /// Low-value customers (bottom 20% by revenue)
    /// </summary>
    [Range(0, int.MaxValue)]
    public int LowValueCustomers { get; init; }

    /// <summary>
    /// Frequent buyers (more than average order frequency)
    /// </summary>
    [Range(0, int.MaxValue)]
    public int FrequentBuyers { get; init; }

    /// <summary>
    /// Occasional buyers (average order frequency)
    /// </summary>
    [Range(0, int.MaxValue)]
    public int OccasionalBuyers { get; init; }

    /// <summary>
    /// Rare buyers (below average order frequency)
    /// </summary>
    [Range(0, int.MaxValue)]
    public int RareBuyers { get; init; }

    /// <summary>
    /// At-risk customers (haven't ordered recently)
    /// </summary>
    [Range(0, int.MaxValue)]
    public int AtRiskCustomers { get; init; }

    /// <summary>
    /// Churned customers (no orders in extended period)
    /// </summary>
    [Range(0, int.MaxValue)]
    public int ChurnedCustomers { get; init; }
}

/// <summary>
/// DTO for customer geographic distribution
/// </summary>
public sealed class CustomerGeographyDto
{
    /// <summary>
    /// Region name
    /// </summary>
    [Required]
    public string Region { get; init; } = string.Empty;

    /// <summary>
    /// Number of customers in the region
    /// </summary>
    [Range(0, int.MaxValue)]
    public int CustomerCount { get; init; }

    /// <summary>
    /// Percentage of total customers
    /// </summary>
    [Range(0, 100)]
    public decimal Percentage { get; init; }

    /// <summary>
    /// Total revenue from the region
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal TotalRevenue { get; init; }

    /// <summary>
    /// Average customer value in the region
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal AverageCustomerValue { get; init; }
}
