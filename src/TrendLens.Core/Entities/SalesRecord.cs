using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrendLens.Core.Entities;

/// <summary>
/// Represents a sales record in the TrendLens system
/// </summary>
[Table("SalesRecords")]
public sealed class SalesRecord
{
    /// <summary>
    /// Unique identifier for the sales record
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    /// <summary>
    /// Date when the sale occurred
    /// </summary>
    [Required]
    [Column(TypeName = "date")]
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Name of the product sold
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Product name must be between 1 and 200 characters")]
    [Column(TypeName = "nvarchar(200)")]
    public string ProductName { get; set; } = string.Empty;
    
    /// <summary>
    /// Category of the product
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Category must be between 1 and 100 characters")]
    [Column(TypeName = "nvarchar(100)")]
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// Sale amount in decimal format
    /// </summary>
    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Quantity of items sold
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
    
    /// <summary>
    /// Name of the customer
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Customer name must be between 1 and 200 characters")]
    [Column(TypeName = "nvarchar(200)")]
    public string CustomerName { get; set; } = string.Empty;
    
    /// <summary>
    /// Geographic region of the sale
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Region must be between 1 and 100 characters")]
    [Column(TypeName = "nvarchar(100)")]
    public string Region { get; set; } = string.Empty;
    
    /// <summary>
    /// Sales representative responsible for the sale
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Sales rep must be between 1 and 200 characters")]
    [Column(TypeName = "nvarchar(200)")]
    public string SalesRep { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the record was created
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the record was last updated
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Calculates the total revenue for this sales record
    /// </summary>
    [NotMapped]
    public decimal TotalRevenue => Amount * Quantity;

    /// <summary>
    /// Gets a formatted display string for the sales record
    /// </summary>
    [NotMapped]
    public string DisplayName => $"{ProductName} - {CustomerName} ({Date:yyyy-MM-dd})";
}
