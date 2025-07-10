using Microsoft.EntityFrameworkCore;
using TrendLens.Application.Services;
using TrendLens.Core.Entities;
using TrendLens.Infrastructure.Data;
using TrendLens.Infrastructure.Repositories;
using Xunit;

namespace TrendLens.Tests.Unit;

public class AnalyticsServiceTests
{
    private TrendLensDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<TrendLensDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new TrendLensDbContext(options);
    }

    private async Task<TrendLensDbContext> SeedTestDataAsync()
    {
        var context = GetInMemoryContext();
        
        var testData = new List<SalesRecord>
        {
            new() { Id = 1, Date = DateTime.Now.AddDays(-30), ProductName = "Product A", Category = "Electronics", Amount = 100m, Quantity = 1, CustomerName = "Customer 1", Region = "North", SalesRep = "Rep 1" },
            new() { Id = 2, Date = DateTime.Now.AddDays(-20), ProductName = "Product B", Category = "Electronics", Amount = 200m, Quantity = 2, CustomerName = "Customer 2", Region = "South", SalesRep = "Rep 2" }
        };

        await context.SalesRecords.AddRangeAsync(testData);
        await context.SaveChangesAsync();
        
        return context;
    }

    [Fact]
    public async Task GetSalesAnalyticsAsync_ReturnsCorrectAnalytics()
    {
        using var context = await SeedTestDataAsync();
        var repository = new SalesRepository(context);
        var service = new AnalyticsService(repository);

        var result = await service.GetSalesAnalyticsAsync();

        Assert.Equal(300m, result.TotalRevenue);
        Assert.Equal(2, result.TotalSales);
        Assert.Equal(150m, result.AverageOrderValue);
    }

    [Fact]
    public async Task GetTopProductsAsync_ReturnsTopProducts()
    {
        using var context = await SeedTestDataAsync();
        var repository = new SalesRepository(context);
        var service = new AnalyticsService(repository);

        var result = await service.GetTopProductsAsync(1);

        Assert.Single(result);
        Assert.Equal("Product B", result.First().ProductName);
        Assert.Equal(200m, result.First().Revenue);
    }
}
