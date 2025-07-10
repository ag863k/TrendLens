ahusing Microsoft.Extensions.Logging;
using System.Globalization;
using TrendLens.Core.Common;
using TrendLens.Core.DTOs;
using TrendLens.Core.Interfaces;

namespace TrendLens.Application.Services;

public class AnalyticsServiceV2 : IAnalyticsServiceV2
{
    private readonly ISalesRepository _salesRepository;
    private readonly ILogger<AnalyticsServiceV2> _logger;

    public AnalyticsServiceV2(ISalesRepository salesRepository, ILogger<AnalyticsServiceV2> logger)
    {
        _salesRepository = salesRepository;
        _logger = logger;
    }

    public async Task<SalesAnalyticsDto> GetSalesAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var result = await GetAdvancedAnalyticsAsync(startDate, endDate);
        return result.IsSuccess ? result.Data! : new SalesAnalyticsDto();
    }

    public async Task<Result<SalesAnalyticsDto>> GetAdvancedAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var sales = startDate.HasValue && endDate.HasValue
                ? await _salesRepository.GetByDateRangeAsync(startDate.Value, endDate.Value)
                : await _salesRepository.GetAllAsync();

            var salesList = sales.ToList();

            var analytics = new SalesAnalyticsDto
            {
                TotalRevenue = salesList.Sum(s => s.Amount),
                TotalSales = salesList.Count,
                AverageOrderValue = salesList.Any() ? salesList.Average(s => s.Amount) : 0,
                MonthlyTrends = (await GetMonthlyTrendsAsync()).ToList(),
                TopProducts = (await GetTopProductsAsync()).ToList()
            };

            return Result<SalesAnalyticsDto>.Success(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sales analytics");
            return Result<SalesAnalyticsDto>.Failure("Failed to generate analytics");
        }
    }

    public async Task<IEnumerable<MonthlyTrendDto>> GetMonthlyTrendsAsync(int months = 12)
    {
        try
        {
            var startDate = DateTime.Now.AddMonths(-months);
            var sales = await _salesRepository.GetByDateRangeAsync(startDate, DateTime.Now);

            return sales.GroupBy(s => new { s.Date.Year, s.Date.Month })
                .Select(g => new MonthlyTrendDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(s => s.Amount),
                    SalesCount = g.Count()
                })
                .OrderBy(t => t.Year)
                .ThenBy(t => t.Month);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating monthly trends");
            return Enumerable.Empty<MonthlyTrendDto>();
        }
    }

    public async Task<IEnumerable<ProductPerformanceDto>> GetTopProductsAsync(int count = 10)
    {
        try
        {
            var sales = await _salesRepository.GetAllAsync();

            return sales.GroupBy(s => s.ProductName)
                .Select(g => new ProductPerformanceDto
                {
                    ProductName = g.Key,
                    Revenue = g.Sum(s => s.Amount),
                    UnitsSold = g.Sum(s => s.Quantity)
                })
                .OrderByDescending(p => p.Revenue)
                .Take(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating top products");
            return Enumerable.Empty<ProductPerformanceDto>();
        }
    }

    public async Task<Result<IEnumerable<RegionalPerformanceDto>>> GetRegionalPerformanceAsync()
    {
        try
        {
            var sales = await _salesRepository.GetAllAsync();
            var currentYearSales = sales.Where(s => s.Date.Year == DateTime.Now.Year).ToList();
            var lastYearSales = sales.Where(s => s.Date.Year == DateTime.Now.Year - 1).ToList();

            var regionalPerformance = currentYearSales
                .GroupBy(s => s.Region)
                .Select(g =>
                {
                    var lastYearRevenue = lastYearSales.Where(s => s.Region == g.Key).Sum(s => s.Amount);
                    var currentRevenue = g.Sum(s => s.Amount);
                    var growthPercentage = lastYearRevenue > 0 
                        ? ((currentRevenue - lastYearRevenue) / lastYearRevenue) * 100 
                        : 0;

                    return new RegionalPerformanceDto
                    {
                        Region = g.Key,
                        TotalRevenue = currentRevenue,
                        OrderCount = g.Count(),
                        AverageOrderValue = g.Average(s => s.Amount),
                        GrowthRate = growthPercentage,
                        CustomerCount = g.Select(s => s.CustomerName).Distinct().Count(),
                        MarketShare = 0 // Will be calculated later
                    };
                })
                .OrderByDescending(r => r.TotalRevenue);

            return Result<IEnumerable<RegionalPerformanceDto>>.Success(regionalPerformance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating regional performance");
            return Result<IEnumerable<RegionalPerformanceDto>>.Failure("Failed to generate regional performance");
        }
    }

    public async Task<Result<IEnumerable<SalesRepPerformanceDto>>> GetSalesRepPerformanceAsync()
    {
        try
        {
            var sales = await _salesRepository.GetAllAsync();

            var salesRepPerformance = sales
                .GroupBy(s => s.SalesRep)
                .Select((g, index) => new SalesRepPerformanceDto
                {
                    SalesRep = g.Key,
                    Region = g.First().Region, // Assuming sales rep works in one region primarily
                    TotalRevenue = g.Sum(s => s.Amount),
                    OrderCount = g.Count(),
                    AverageOrderValue = g.Average(s => s.Amount),
                    CustomerCount = g.Select(s => s.CustomerName).Distinct().Count(),
                    ConversionRate = 100, // Default value, would need more data to calculate actual conversion
                    TargetAchievement = 100, // Default value, would need target data
                    Rank = index + 1,
                    GrowthRate = 0, // Would need historical data to calculate
                    Commission = g.Sum(s => s.Amount) * 0.05m // Assuming 5% commission
                })
                .OrderByDescending(r => r.TotalRevenue);

            return Result<IEnumerable<SalesRepPerformanceDto>>.Success(salesRepPerformance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sales rep performance");
            return Result<IEnumerable<SalesRepPerformanceDto>>.Failure("Failed to generate sales rep performance");
        }
    }

    public async Task<Result<IEnumerable<SalesTrendDto>>> GetSalesTrendsAsync(string period = "monthly", int count = 12)
    {
        try
        {
            _logger.LogInformation("Getting sales trends for period {Period}, count {Count}", period, count);
            
            var sales = await _salesRepository.GetAllAsync();
            var trends = new List<SalesTrendDto>();

            var endDate = DateTime.Now.Date;
            var startDate = period.ToLower() switch
            {
                "daily" => endDate.AddDays(-count),
                "weekly" => endDate.AddDays(-count * 7),
                "monthly" => endDate.AddMonths(-count),
                "quarterly" => endDate.AddMonths(-count * 3),
                "yearly" => endDate.AddYears(-count),
                _ => endDate.AddMonths(-count)
            };

            var filteredSales = sales.Where(s => s.Date >= startDate && s.Date <= endDate)
                                   .OrderBy(s => s.Date)
                                   .ToList();

            // Group sales by period
            IEnumerable<IGrouping<object, Core.Entities.SalesRecord>> groupedSales = period.ToLower() switch
            {
                "daily" => filteredSales.GroupBy(s => (object)s.Date.Date),
                "weekly" => filteredSales.GroupBy(s => (object)CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(s.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday)),
                "monthly" => filteredSales.GroupBy(s => (object)new { s.Date.Year, s.Date.Month }),
                "quarterly" => filteredSales.GroupBy(s => (object)new { s.Date.Year, Quarter = (s.Date.Month - 1) / 3 + 1 }),
                "yearly" => filteredSales.GroupBy(s => (object)s.Date.Year),
                _ => filteredSales.GroupBy(s => (object)new { s.Date.Year, s.Date.Month })
            };

            foreach (var group in groupedSales.Take(count))
            {
                var groupSales = group.ToList();
                var revenue = groupSales.Sum(s => s.TotalRevenue);
                var orderCount = groupSales.Count;
                var avgOrderValue = orderCount > 0 ? revenue / orderCount : 0;

                var trend = new SalesTrendDto
                {
                    Period = FormatPeriod(group, period),
                    PeriodType = period,
                    StartDate = groupSales.Min(s => s.Date),
                    EndDate = groupSales.Max(s => s.Date),
                    Revenue = revenue,
                    OrderCount = orderCount,
                    AverageOrderValue = avgOrderValue,
                    TrendDirection = "stable",
                    TrendStrength = "moderate"
                };

                trends.Add(trend);
            }

            return Result<IEnumerable<SalesTrendDto>>.Success(trends);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sales trends");
            return Result<IEnumerable<SalesTrendDto>>.Failure(ex);
        }
    }

    public async Task<Result<CustomerAnalyticsDto>> GetCustomerAnalyticsAsync()
    {
        try
        {
            _logger.LogInformation("Getting customer analytics");
            
            var sales = await _salesRepository.GetAllAsync();
            var customers = sales.GroupBy(s => s.CustomerName).ToList();

            var totalCustomers = customers.Count;
            var totalRevenue = sales.Sum(s => s.TotalRevenue);
            var totalOrders = sales.Count();

            // Calculate customer metrics
            var customerLifetimeValues = customers.Select(g => new
            {
                CustomerName = g.Key,
                TotalRevenue = g.Sum(s => s.TotalRevenue),
                OrderCount = g.Count(),
                FirstOrder = g.Min(s => s.Date),
                LastOrder = g.Max(s => s.Date)
            }).ToList();

            var avgCLV = customerLifetimeValues.Any() ? customerLifetimeValues.Average(c => c.TotalRevenue) : 0;
            var avgOrderFrequency = customerLifetimeValues.Any() ? customerLifetimeValues.Average(c => c.OrderCount) : 0;
            var avgOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            // Top customers
            var topCustomers = customerLifetimeValues
                .OrderByDescending(c => c.TotalRevenue)
                .Take(10)
                .Select((c, index) => new TopCustomerDto
                {
                    CustomerName = c.CustomerName,
                    TotalRevenue = c.TotalRevenue,
                    OrderCount = c.OrderCount,
                    AverageOrderValue = c.OrderCount > 0 ? c.TotalRevenue / c.OrderCount : 0,
                    LastOrderDate = c.LastOrder,
                    LifetimeValue = c.TotalRevenue,
                    Rank = index + 1
                });

            // Geographic distribution
            var geographicDistribution = sales.GroupBy(s => s.Region)
                .Select(g => new CustomerGeographyDto
                {
                    Region = g.Key,
                    CustomerCount = g.Select(s => s.CustomerName).Distinct().Count(),
                    Percentage = totalCustomers > 0 ? (decimal)g.Select(s => s.CustomerName).Distinct().Count() / totalCustomers * 100 : 0,
                    TotalRevenue = g.Sum(s => s.TotalRevenue),
                    AverageCustomerValue = g.Select(s => s.CustomerName).Distinct().Count() > 0 
                        ? g.Sum(s => s.TotalRevenue) / g.Select(s => s.CustomerName).Distinct().Count() 
                        : 0
                });

            var analytics = new CustomerAnalyticsDto
            {
                TotalCustomers = totalCustomers,
                NewCustomers = 0, // Would need date-based logic to calculate
                ReturningCustomers = customerLifetimeValues.Count(c => c.OrderCount > 1),
                RetentionRate = totalCustomers > 0 ? (decimal)customerLifetimeValues.Count(c => c.OrderCount > 1) / totalCustomers * 100 : 0,
                AverageCustomerLifetimeValue = avgCLV,
                AverageOrderFrequency = (decimal)avgOrderFrequency,
                AverageOrderValue = avgOrderValue,
                CustomerAcquisitionCost = 0, // Would need marketing cost data
                ChurnRate = 0, // Would need time-based churn analysis
                TopCustomers = topCustomers,
                Segmentation = new CustomerSegmentationDto
                {
                    HighValueCustomers = customerLifetimeValues.Count(c => c.TotalRevenue >= avgCLV * 2),
                    MediumValueCustomers = customerLifetimeValues.Count(c => c.TotalRevenue >= avgCLV * 0.5m && c.TotalRevenue < avgCLV * 2),
                    LowValueCustomers = customerLifetimeValues.Count(c => c.TotalRevenue < avgCLV * 0.5m),
                    FrequentBuyers = customerLifetimeValues.Count(c => c.OrderCount > avgOrderFrequency),
                    OccasionalBuyers = customerLifetimeValues.Count(c => c.OrderCount == avgOrderFrequency),
                    RareBuyers = customerLifetimeValues.Count(c => c.OrderCount < avgOrderFrequency)
                },
                GeographicDistribution = geographicDistribution
            };

            return Result<CustomerAnalyticsDto>.Success(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer analytics");
            return Result<CustomerAnalyticsDto>.Failure(ex);
        }
    }

    private static string FormatPeriod(IGrouping<object, Core.Entities.SalesRecord> group, string period)
    {
        return period.ToLower() switch
        {
            "daily" => ((DateTime)group.Key).ToString("yyyy-MM-dd"),
            "weekly" => $"Week {group.Key}",
            "monthly" => $"{((dynamic)group.Key).Year}-{((dynamic)group.Key).Month:D2}",
            "quarterly" => $"{((dynamic)group.Key).Year}-Q{((dynamic)group.Key).Quarter}",
            "yearly" => group.Key.ToString() ?? "Unknown",
            _ => group.Key.ToString() ?? "Unknown"
        };
    }
}
