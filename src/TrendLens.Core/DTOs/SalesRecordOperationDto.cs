using System.ComponentModel.DataAnnotations;

namespace TrendLens.Core.DTOs;

/// <summary>
/// DTO for creating a new sales record
/// </summary>
public sealed class CreateSalesRecordDto
{
    /// <summary>
    /// Date of the sale
    /// </summary>
    [Required]
    public DateTime Date { get; init; }

    /// <summary>
    /// Name of the product sold
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string ProductName { get; init; } = string.Empty;

    /// <summary>
    /// Product category
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Sale amount per unit
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; init; }

    /// <summary>
    /// Quantity sold
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }

    /// <summary>
    /// Customer name
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string CustomerName { get; init; } = string.Empty;

    /// <summary>
    /// Sales region
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Region { get; init; } = string.Empty;

    /// <summary>
    /// Sales representative
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string SalesRep { get; init; } = string.Empty;
}

/// <summary>
/// DTO for updating an existing sales record
/// </summary>
public sealed class UpdateSalesRecordDto
{
    /// <summary>
    /// ID of the sales record to update
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int Id { get; init; }

    /// <summary>
    /// Date of the sale
    /// </summary>
    [Required]
    public DateTime Date { get; init; }

    /// <summary>
    /// Name of the product sold
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string ProductName { get; init; } = string.Empty;

    /// <summary>
    /// Product category
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Sale amount per unit
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; init; }

    /// <summary>
    /// Quantity sold
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }

    /// <summary>
    /// Customer name
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string CustomerName { get; init; } = string.Empty;

    /// <summary>
    /// Sales region
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Region { get; init; } = string.Empty;

    /// <summary>
    /// Sales representative
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string SalesRep { get; init; } = string.Empty;
}
