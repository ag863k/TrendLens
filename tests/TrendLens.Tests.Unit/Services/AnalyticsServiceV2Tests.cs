using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrendLens.Application.Services;
using TrendLens.Core.Entities;
using TrendLens.Infrastructure.Data;
using TrendLens.Infrastructure.Repositories;
using Xunit;
using Moq;

namespace TrendLens.Tests.Unit.Services;

public class AnalyticsServiceV2Tests : IDisposable
{
    private readonly TrendLensDbContext _context;
    private readonly SalesRepository _repository;
    private readonly Mock<ILogger<AnalyticsServiceV2>> _mockLogger;
    private readonly AnalyticsServiceV2 _service;

    public AnalyticsServiceV2Tests()
    {
        var options = new DbContextOptionsBuilder<TrendLensDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrendLensDbContext(options);
        _mockLogger = new Mock<ILogger<AnalyticsServiceV2>>();
        var repoLogger = new Mock<ILogger<SalesRepository>>();
        _repository = new SalesRepository(_context, repoLogger.Object);
        _service = new AnalyticsServiceV2(_repository, _mockLogger.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var testData = new List<SalesRecord>
        {
            new() 
            { 
                Id = 1, 
                Date = DateTime.Now.AddDays(-30), 
                ProductName = "Laptop Pro", 
                Category = "Electronics", 
                Amount = 1299.99m, 
                Quantity = 1, 
                CustomerName = "Tech Corp", 
                Region = "North", 
                SalesRep = "John Smith" 
            },
            new() 
            { 
                Id = 2, 
                Date = DateTime.Now.AddDays(-20), 
                ProductName = "Wireless Mouse", 
                Category = "Electronics", 
                Amount = 29.99m, 
                Quantity = 5, 
                CustomerName = "Small Business", 
                Region = "South", 
                SalesRep = "Jane Doe" 
            },
            new() 
            { 
                Id = 3, 
                Date = DateTime.Now.AddDays(-10), 
                ProductName = "Office Chair", 
                Category = "Furniture", 
                Amount = 299.99m, 
                Quantity = 2, 
                CustomerName = "Enterprise Inc", 
                Region = "East", 
                SalesRep = "Bob Wilson" 
            }
        };

        _context.SalesRecords.AddRange(testData);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAdvancedAnalyticsAsync_ReturnsSuccessResult()
    {
        var result = await _service.GetAdvancedAnalyticsAsync();

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(1629.97m, result.Data.TotalRevenue);
        Assert.Equal(3, result.Data.TotalSales);
    }

    [Fact]
    public async Task GetRegionalPerformanceAsync_ReturnsCorrectData()
    {
        var result = await _service.GetRegionalPerformanceAsync();

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Contains(result.Data, r => r.Region == "North");
        Assert.Contains(result.Data, r => r.Region == "South");
        Assert.Contains(result.Data, r => r.Region == "East");
    }

    [Fact]
    public async Task GetSalesRepPerformanceAsync_ReturnsCorrectData()
    {
        var result = await _service.GetSalesRepPerformanceAsync();

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Contains(result.Data, r => r.SalesRep == "John Smith");
        Assert.Contains(result.Data, r => r.SalesRep == "Jane Doe");
        Assert.Contains(result.Data, r => r.SalesRep == "Bob Wilson");
    }

    [Fact]
    public async Task GetTopProductsAsync_ReturnsOrderedByRevenue()
    {
        var products = await _service.GetTopProductsAsync(10);

        var productsList = products.ToList();
        Assert.NotEmpty(productsList);
        
        // Should be ordered by revenue descending
        for (int i = 0; i < productsList.Count - 1; i++)
        {
            Assert.True(productsList[i].Revenue >= productsList[i + 1].Revenue);
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
