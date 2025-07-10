using Microsoft.EntityFrameworkCore;
using TrendLens.Core.Entities;

namespace TrendLens.Infrastructure.Data;

public class TrendLensDbContext : DbContext
{
    public TrendLensDbContext(DbContextOptions<TrendLensDbContext> options) : base(options) { }

    public DbSet<SalesRecord> SalesRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SalesRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CustomerName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Region).HasMaxLength(100).IsRequired();
            entity.Property(e => e.SalesRep).HasMaxLength(200).IsRequired();
            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Region);
        });

        base.OnModelCreating(modelBuilder);
    }
}
