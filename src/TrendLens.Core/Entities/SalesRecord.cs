using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrendLens.Core.Entities;

[Table("SalesRecords")]
public sealed class SalesRecord
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [Column(TypeName = "date")]
    public DateTime Date { get; set; }
    
    [Required]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Product name must be between 1 and 200 characters")]
    [Column(TypeName = "nvarchar(200)")]
    public string ProductName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Category must be between 1 and 100 characters")]
    [Column(TypeName = "nvarchar(100)")]
    public string Category { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
    
    [Required]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Customer name must be between 1 and 200 characters")]
    [Column(TypeName = "nvarchar(200)")]
    public string CustomerName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Region must be between 1 and 100 characters")]
    [Column(TypeName = "nvarchar(100)")]
    public string Region { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Sales rep must be between 1 and 200 characters")]
    [Column(TypeName = "nvarchar(200)")]
    public string SalesRep { get; set; } = string.Empty;

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public decimal TotalRevenue => Amount * Quantity;

    [NotMapped]
    public string DisplayName => $"{ProductName} - {CustomerName} ({Date:yyyy-MM-dd})";
}
