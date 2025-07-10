using TrendLens.Core.Common;
using TrendLens.Core.DTOs;
using TrendLens.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace TrendLens.Core.Interfaces;

public interface ISalesRepository
{
    Task<IEnumerable<SalesRecord>> GetAllAsync();
    Task<SalesRecord?> GetByIdAsync(int id);
    Task<SalesRecord> AddAsync(SalesRecord salesRecord);
    Task<IEnumerable<SalesRecord>> AddRangeAsync(IEnumerable<SalesRecord> salesRecords);
    Task UpdateAsync(SalesRecord salesRecord);
    Task DeleteAsync(int id);
    Task<IEnumerable<SalesRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
}

public interface IAnalyticsService
{
    Task<SalesAnalyticsDto> GetSalesAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<MonthlyTrendDto>> GetMonthlyTrendsAsync(int months = 12);
    Task<IEnumerable<ProductPerformanceDto>> GetTopProductsAsync(int count = 10);
}

public interface IMachineLearningService
{
    Task<IEnumerable<SalesForecastDto>> GetSalesForecastAsync(int days = 30);
    Task<decimal> PredictCustomerLifetimeValueAsync(string customerName);
    Task TrainModelAsync();
}

public interface ICsvImportService
{
    Task<IEnumerable<SalesRecord>> ImportFromCsvAsync(Stream csvStream);
    bool ValidateCsvFormat(Stream csvStream);
}

/// <summary>
/// Interface for sales-related operations
/// </summary>
public interface ISalesService
{
    /// <summary>
    /// Retrieves all sales records
    /// </summary>
    Task<Result<IEnumerable<SalesRecordDto>>> GetAllSalesAsync();

    /// <summary>
    /// Retrieves a specific sales record by ID
    /// </summary>
    Task<Result<SalesRecordDto>> GetSalesByIdAsync(int id);

    /// <summary>
    /// Creates a new sales record
    /// </summary>
    Task<Result<SalesRecordDto>> CreateSalesAsync(CreateSalesRecordDto createDto);

    /// <summary>
    /// Updates an existing sales record
    /// </summary>
    Task<Result<SalesRecordDto>> UpdateSalesAsync(UpdateSalesRecordDto updateDto);

    /// <summary>
    /// Deletes a sales record by ID
    /// </summary>
    Task<Result> DeleteSalesAsync(int id);

    /// <summary>
    /// Imports sales data from CSV
    /// </summary>
    Task<Result<int>> ImportCsvAsync(Stream csvStream, string fileName);

    /// <summary>
    /// Searches for sales records based on criteria
    /// </summary>
    Task<Result<PaginatedResultDto<SalesRecordDto>>> SearchSalesAsync(SalesRecordSearchDto searchDto);

    /// <summary>
    /// Gets sales records by date range
    /// </summary>
    Task<Result<IEnumerable<SalesRecordDto>>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
}

/// <summary>
/// Enhanced analytics service interface
/// </summary>
public interface IAnalyticsServiceV2 : IAnalyticsService
{
    /// <summary>
    /// Gets advanced analytics with additional metrics
    /// </summary>
    Task<Result<SalesAnalyticsDto>> GetAdvancedAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Gets performance metrics by region
    /// </summary>
    Task<Result<IEnumerable<RegionalPerformanceDto>>> GetRegionalPerformanceAsync();

    /// <summary>
    /// Gets performance metrics by sales representative
    /// </summary>
    Task<Result<IEnumerable<SalesRepPerformanceDto>>> GetSalesRepPerformanceAsync();

    /// <summary>
    /// Gets sales trends over time
    /// </summary>
    Task<Result<IEnumerable<SalesTrendDto>>> GetSalesTrendsAsync(string period = "monthly", int count = 12);

    /// <summary>
    /// Gets customer analytics
    /// </summary>
    Task<Result<CustomerAnalyticsDto>> GetCustomerAnalyticsAsync();
}
