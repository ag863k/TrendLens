using System.ComponentModel.DataAnnotations;

namespace TrendLens.Core.DTOs;

/// <summary>
/// Data transfer object for sales record information
/// </summary>
public sealed record SalesRecordDto
{
    /// <summary>
    /// Unique identifier for the sales record
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Date when the sale occurred
    /// </summary>
    public DateTime Date { get; init; }

    /// <summary>
    /// Formatted date string for display
    /// </summary>
    public string DateFormatted => Date.ToString("yyyy-MM-dd");

    /// <summary>
    /// Name of the product sold
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string ProductName { get; init; } = string.Empty;

    /// <summary>
    /// Category of the product
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Sale amount per unit
    /// </summary>
    [Range(0.01, (double)decimal.MaxValue)]
    public decimal Amount { get; init; }

    /// <summary>
    /// Quantity of items sold
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }

    /// <summary>
    /// Total value of the sale (Amount * Quantity)
    /// </summary>
    public decimal TotalValue => Amount * Quantity;

    /// <summary>
    /// Name of the customer
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string CustomerName { get; init; } = string.Empty;

    /// <summary>
    /// Geographic region of the sale
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Region { get; init; } = string.Empty;

    /// <summary>
    /// Sales representative responsible for the sale
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string SalesRep { get; init; } = string.Empty;

    /// <summary>
    /// Timestamp when the record was created
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Timestamp when the record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; init; }
}

/// <summary>
/// Data transfer object for creating a new sales record
/// </summary>
public record CreateSalesRecordDto
{
    /// <summary>
    /// Date when the sale occurred
    /// </summary>
    [Required]
    public DateTime Date { get; init; }

    /// <summary>
    /// Name of the product sold
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Product name must be between 1 and 200 characters")]
    public string ProductName { get; init; } = string.Empty;

    /// <summary>
    /// Category of the product
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Category must be between 1 and 100 characters")]
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Sale amount per unit
    /// </summary>
    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; init; }

    /// <summary>
    /// Quantity of items sold
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; init; }

    /// <summary>
    /// Name of the customer
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Customer name must be between 1 and 200 characters")]
    public string CustomerName { get; init; } = string.Empty;

    /// <summary>
    /// Geographic region of the sale
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Region must be between 1 and 100 characters")]
    public string Region { get; init; } = string.Empty;

    /// <summary>
    /// Sales representative responsible for the sale
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Sales rep must be between 1 and 200 characters")]
    public string SalesRep { get; init; } = string.Empty;
}

/// <summary>
/// Data transfer object for updating an existing sales record
/// </summary>
public sealed record UpdateSalesRecordDto : CreateSalesRecordDto
{
    /// <summary>
    /// Identifier of the sales record to update
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be a positive number")]
    public int Id { get; init; }
}

/// <summary>
/// Data transfer object for sales record search/filter criteria
/// </summary>
public sealed record SalesRecordSearchDto
{
    /// <summary>
    /// Filter by product name (partial match)
    /// </summary>
    public string? ProductName { get; init; }

    /// <summary>
    /// Filter by category
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Filter by customer name (partial match)
    /// </summary>
    public string? CustomerName { get; init; }

    /// <summary>
    /// Filter by region
    /// </summary>
    public string? Region { get; init; }

    /// <summary>
    /// Filter by sales representative
    /// </summary>
    public string? SalesRep { get; init; }

    /// <summary>
    /// Filter by date range - start date
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// Filter by date range - end date
    /// </summary>
    public DateTime? EndDate { get; init; }

    /// <summary>
    /// Minimum amount filter
    /// </summary>
    [Range(0, (double)decimal.MaxValue)]
    public decimal? MinAmount { get; init; }

    /// <summary>
    /// Maximum amount filter
    /// </summary>
    [Range(0, (double)decimal.MaxValue)]
    public decimal? MaxAmount { get; init; }

    /// <summary>
    /// Page number for pagination (1-based)
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    /// <summary>
    /// Page size for pagination
    /// </summary>
    [Range(1, 1000)]
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Sort field
    /// </summary>
    public string? SortBy { get; init; }

    /// <summary>
    /// Sort direction (asc/desc)
    /// </summary>
    public string SortDirection { get; init; } = "asc";
}
