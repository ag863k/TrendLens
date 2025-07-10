using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using TrendLens.Core.Common;
using TrendLens.Core.Configuration;
using TrendLens.Core.DTOs;
using TrendLens.Core.Interfaces;

namespace TrendLens.Application.Services;

/// <summary>
/// Enterprise-grade machine learning service with robust error handling and caching
/// </summary>
public sealed class MachineLearningService : IMachineLearningService
{
    private readonly ISalesRepository _salesRepository;
    private readonly HttpClient _httpClient;
    private readonly MLServiceConfiguration _mlConfig;
    private readonly FeaturesConfiguration _featuresConfig;
    private readonly ILogger<MachineLearningService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = false
    };

    public MachineLearningService(
        ISalesRepository salesRepository,
        HttpClient httpClient,
        IOptions<MLServiceConfiguration> mlConfig,
        IOptions<FeaturesConfiguration> featuresConfig,
        ILogger<MachineLearningService> logger)
    {
        _salesRepository = salesRepository ?? throw new ArgumentNullException(nameof(salesRepository));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _mlConfig = mlConfig?.Value ?? throw new ArgumentNullException(nameof(mlConfig));
        _featuresConfig = featuresConfig?.Value ?? throw new ArgumentNullException(nameof(featuresConfig));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        ConfigureHttpClient();
    }

    /// <summary>
    /// Configures the HTTP client with timeout and authentication
    /// </summary>
    private void ConfigureHttpClient()
    {
        _httpClient.Timeout = TimeSpan.FromSeconds(_mlConfig.Timeout);
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", _mlConfig.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "TrendLens/1.0");
    }

    /// <summary>
    /// Gets sales forecast with ML service fallback
    /// </summary>
    public async Task<IEnumerable<SalesForecastDto>> GetSalesForecastAsync(int days = 30)
    {
        using var activity = _logger.BeginScope("Getting sales forecast for {Days} days", days);
        
        try
        {
            if (days <= 0 || days > 365)
            {
                throw new ArgumentOutOfRangeException(nameof(days), "Days must be between 1 and 365");
            }

            if (_featuresConfig.EnableMLService && !string.IsNullOrEmpty(_mlConfig.BaseUrl))
            {
                var mlResult = await TryGetForecastFromMLServiceAsync(days);
                if (mlResult.IsSuccess)
                {
                    _logger.LogInformation("Successfully retrieved forecast from ML service for {Days} days", days);
                    return mlResult.Data!;
                }

                _logger.LogWarning("ML service failed: {Error}. Falling back to statistical forecast", mlResult.Error);
            }

            var statisticalResult = await GetStatisticalForecastAsync(days);
            return statisticalResult.IsSuccess ? statisticalResult.Data! : Enumerable.Empty<SalesForecastDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error generating sales forecast for {Days} days", days);
            return await CreateEmergencyForecastAsync(days);
        }
    }

    /// <summary>
    /// Attempts to get forecast from ML service with retry logic
    /// </summary>
    private async Task<Result<IEnumerable<SalesForecastDto>>> TryGetForecastFromMLServiceAsync(int days)
    {
        for (int attempt = 1; attempt <= _mlConfig.RetryAttempts; attempt++)
        {
            try
            {
                var salesResult = await _salesRepository.GetAllAsync();
                var salesData = salesResult.Select(CreateMLSalesData).ToList();

                if (!salesData.Any())
                {
                    return Result<IEnumerable<SalesForecastDto>>.Failure("No sales data available for ML forecasting");
                }

                var request = new MLForecastRequest
                {
                    SalesData = salesData,
                    ForecastDays = days,
                    IncludeConfidenceIntervals = true,
                    ModelType = "advanced_timeseries"
                };

                var result = await CallMLServiceAsync<MLForecastRequest, MLForecastResponse>(
                    "/api/v1/forecast", request);

                if (result.IsSuccess && result.Data?.Forecasts != null)
                {
                    var forecasts = result.Data.Forecasts.Select(MapToSalesForecastDto);
                    return Result<IEnumerable<SalesForecastDto>>.Success(forecasts);
                }

                if (attempt < _mlConfig.RetryAttempts)
                {
                    var delay = _mlConfig.RetryDelay * attempt;
                    _logger.LogWarning("ML service attempt {Attempt} failed, retrying in {Delay}ms", attempt, delay);
                    await Task.Delay(delay);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ML service attempt {Attempt} failed", attempt);
                
                if (attempt == _mlConfig.RetryAttempts)
                {
                    return Result<IEnumerable<SalesForecastDto>>.Failure(ex, "All ML service attempts failed");
                }

                await Task.Delay(_mlConfig.RetryDelay * attempt);
            }
        }

        return Result<IEnumerable<SalesForecastDto>>.Failure("ML service unavailable after all retry attempts");
    }

    /// <summary>
    /// Creates statistical forecast using advanced time series analysis
    /// </summary>
    private async Task<Result<IEnumerable<SalesForecastDto>>> GetStatisticalForecastAsync(int days)
    {
        try
        {
            var sales = await _salesRepository.GetAllAsync();
            var salesData = sales.OrderBy(s => s.Date).ToList();

            if (!salesData.Any())
            {
                return Result<IEnumerable<SalesForecastDto>>.Success(Enumerable.Empty<SalesForecastDto>());
            }

            var forecasts = new List<SalesForecastDto>();
            var analyzer = new TimeSeriesAnalyzer(salesData, _logger);

            for (int i = 1; i <= days; i++)
            {
                var forecastDate = DateTime.Now.Date.AddDays(i);
                var analysis = analyzer.AnalyzeForDate(forecastDate, i);

                var forecast = new SalesForecastDto
                {
                    Date = forecastDate,
                    PredictedRevenue = Math.Max(0, analysis.PredictedRevenue),
                    ConfidenceInterval = analysis.ConfidenceLevel,
                    LowerBound = Math.Max(0, analysis.LowerBound),
                    UpperBound = analysis.UpperBound,
                    ForecastType = "statistical"
                };

                forecasts.Add(forecast);
            }

            return Result<IEnumerable<SalesForecastDto>>.Success(forecasts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating statistical forecast");
            return Result<IEnumerable<SalesForecastDto>>.Failure(ex);
        }
    }

    /// <summary>
    /// Predicts customer lifetime value with advanced algorithms
    /// </summary>
    public async Task<decimal> PredictCustomerLifetimeValueAsync(string customerName)
    {
        using var activity = _logger.BeginScope("Predicting CLV for customer {CustomerName}", customerName);

        try
        {
            if (string.IsNullOrWhiteSpace(customerName))
            {
                _logger.LogWarning("Customer name is null or empty for CLV prediction");
                return 0;
            }

            // Try ML service first if available
            if (_featuresConfig.EnableMLService && !string.IsNullOrEmpty(_mlConfig.BaseUrl))
            {
                var mlClv = await TryGetCLVFromMLServiceAsync(customerName);
                if (mlClv > 0)
                {
                    _logger.LogInformation("ML service predicted CLV of {CLV:C} for {CustomerName}", mlClv, customerName);
                    return mlClv;
                }
            }

            // Fallback to statistical CLV calculation
            var statisticalClv = await CalculateStatisticalCLVAsync(customerName);
            _logger.LogInformation("Statistical CLV prediction: {CLV:C} for {CustomerName}", statisticalClv, customerName);
            return statisticalClv;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error predicting CLV for customer {CustomerName}", customerName);
            return 0;
        }
    }

    /// <summary>
    /// Attempts to get CLV from ML service
    /// </summary>
    private async Task<decimal> TryGetCLVFromMLServiceAsync(string customerName)
    {
        try
        {
            var sales = await _salesRepository.GetAllAsync();
            var customerSales = sales
                .Where(s => s.CustomerName.Equals(customerName, StringComparison.OrdinalIgnoreCase))
                .OrderBy(s => s.Date)
                .ToList();

            if (!customerSales.Any())
                return 0;

            var request = new MLCLVRequest
            {
                CustomerName = customerName,
                SalesHistory = customerSales.Select(CreateMLSalesData).ToList(),
                PredictionHorizonMonths = 24
            };

            var result = await CallMLServiceAsync<MLCLVRequest, MLCLVResponse>("/api/v1/clv", request);
            return result.IsSuccess ? result.Data?.PredictedCLV ?? 0 : 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling ML service for CLV prediction");
            return 0;
        }
    }

    /// <summary>
    /// Calculates CLV using advanced statistical methods
    /// </summary>
    private async Task<decimal> CalculateStatisticalCLVAsync(string customerName)
    {
        var sales = await _salesRepository.GetAllAsync();
        var customerSales = sales
            .Where(s => s.CustomerName.Equals(customerName, StringComparison.OrdinalIgnoreCase))
            .OrderBy(s => s.Date)
            .ToList();

        if (!customerSales.Any())
            return 0;

        var calculator = new CLVCalculator(customerSales, _logger);
        return calculator.CalculateAdvancedCLV();
    }

    /// <summary>
    /// Trains the ML model with current data
    /// </summary>
    public async Task TrainModelAsync()
    {
        using var activity = _logger.BeginScope("Training ML model");

        try
        {
            if (!_featuresConfig.EnableMLService || string.IsNullOrEmpty(_mlConfig.BaseUrl))
            {
                _logger.LogInformation("ML service disabled, skipping model training");
                return;
            }

            var sales = await _salesRepository.GetAllAsync();
            var trainingData = sales.Select(CreateMLTrainingData).ToList();

            if (!trainingData.Any())
            {
                _logger.LogWarning("No training data available");
                return;
            }

            var request = new MLTrainingRequest
            {
                TrainingData = trainingData,
                ModelTypes = new[] { "forecast", "clv", "recommendation" },
                ValidationSplit = 0.2,
                CrossValidationFolds = 5
            };

            var result = await CallMLServiceAsync<MLTrainingRequest, MLTrainingResponse>("/api/v1/train", request);

            if (result.IsSuccess)
            {
                _logger.LogInformation("ML model training completed successfully. Model accuracy: {Accuracy:P}", 
                    result.Data?.Accuracy ?? 0);
            }
            else
            {
                _logger.LogError("ML model training failed: {Error}", result.Error);
                throw new InvalidOperationException($"ML training failed: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error during ML model training");
            throw;
        }
    }

    /// <summary>
    /// Generic method to call ML service endpoints
    /// </summary>
    private async Task<Result<TResponse>> CallMLServiceAsync<TRequest, TResponse>(string endpoint, TRequest request)
        where TResponse : class
    {
        try
        {
            var json = JsonSerializer.Serialize(request, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_mlConfig.BaseUrl.TrimEnd('/')}{endpoint}", content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TResponse>(responseJson, JsonOptions);
                return result != null 
                    ? Result<TResponse>.Success(result)
                    : Result<TResponse>.Failure("Failed to deserialize ML service response");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("ML service returned {StatusCode}: {Error}", response.StatusCode, errorContent);
            return Result<TResponse>.Failure($"ML service error {response.StatusCode}: {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling ML service endpoint {Endpoint}", endpoint);
            return Result<TResponse>.Failure(ex);
        }
    }

    /// <summary>
    /// Creates emergency forecast when all other methods fail
    /// </summary>
    private Task<IEnumerable<SalesForecastDto>> CreateEmergencyForecastAsync(int days)
    {
        try
        {
            _logger.LogWarning("Creating emergency forecast for {Days} days", days);
            
            var forecasts = new List<SalesForecastDto>();
            var baseRevenue = 1000m; // Conservative baseline

            for (int i = 1; i <= days; i++)
            {
                forecasts.Add(new SalesForecastDto
                {
                    Date = DateTime.Now.Date.AddDays(i),
                    PredictedRevenue = baseRevenue,
                    ConfidenceInterval = 50m,
                    LowerBound = baseRevenue * 0.5m,
                    UpperBound = baseRevenue * 1.5m,
                    ForecastType = "emergency"
                });
            }

            return Task.FromResult(forecasts.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create emergency forecast");
            return Task.FromResult(Enumerable.Empty<SalesForecastDto>());
        }
    }

    /// <summary>
    /// Maps sales record to ML service data format
    /// </summary>
    private static MLSalesData CreateMLSalesData(Core.Entities.SalesRecord sale) => new()
    {
        Date = sale.Date.ToString("yyyy-MM-dd"),
        ProductName = sale.ProductName,
        Category = sale.Category,
        Amount = sale.Amount,
        Quantity = sale.Quantity,
        CustomerName = sale.CustomerName,
        Region = sale.Region,
        SalesRep = sale.SalesRep,
        TotalValue = sale.TotalRevenue
    };

    /// <summary>
    /// Maps sales record to ML training data format
    /// </summary>
    private static MLTrainingData CreateMLTrainingData(Core.Entities.SalesRecord sale) => new()
    {
        Date = sale.Date,
        ProductName = sale.ProductName,
        Category = sale.Category,
        Amount = sale.Amount,
        Quantity = sale.Quantity,
        CustomerName = sale.CustomerName,
        Region = sale.Region,
        SalesRep = sale.SalesRep,
        DayOfWeek = sale.Date.DayOfWeek.ToString(),
        Month = sale.Date.Month,
        Quarter = (sale.Date.Month - 1) / 3 + 1,
        IsWeekend = sale.Date.DayOfWeek == DayOfWeek.Saturday || sale.Date.DayOfWeek == DayOfWeek.Sunday
    };

    /// <summary>
    /// Maps ML service forecast to DTO
    /// </summary>
    private static SalesForecastDto MapToSalesForecastDto(MLForecastItem item) => new()
    {
        Date = DateTime.Parse(item.Date),
        PredictedRevenue = item.PredictedValue,
        ConfidenceInterval = item.ConfidenceLevel,
        LowerBound = item.LowerBound,
        UpperBound = item.UpperBound,
        ForecastType = "ml_service"
    };
}

/// <summary>
/// Advanced time series analyzer for statistical forecasting
/// </summary>
internal sealed class TimeSeriesAnalyzer
{
    private readonly List<Core.Entities.SalesRecord> _salesData;
    private readonly ILogger _logger;

    public TimeSeriesAnalyzer(List<Core.Entities.SalesRecord> salesData, ILogger logger)
    {
        _salesData = salesData;
        _logger = logger;
    }

    public (decimal PredictedRevenue, decimal ConfidenceLevel, decimal LowerBound, decimal UpperBound) AnalyzeForDate(DateTime date, int daysAhead)
    {
        var recentData = _salesData.TakeLast(Math.Min(90, _salesData.Count)).ToList();
        
        if (!recentData.Any())
            return (0, 0, 0, 0);

        var baselineRevenue = CalculateBaseline(recentData);
        var seasonalFactor = CalculateSeasonalFactor(date);
        var trendFactor = CalculateTrendFactor(recentData, daysAhead);
        var volatilityFactor = CalculateVolatility(recentData);

        var predictedRevenue = baselineRevenue * seasonalFactor * trendFactor;
        var confidence = Math.Max(50m, 90m - (daysAhead * 2m));
        var margin = predictedRevenue * volatilityFactor;

        return (
            PredictedRevenue: predictedRevenue,
            ConfidenceLevel: confidence,
            LowerBound: predictedRevenue - margin,
            UpperBound: predictedRevenue + margin
        );
    }

    private decimal CalculateBaseline(List<Core.Entities.SalesRecord> data)
    {
        if (!data.Any()) return 0;

        // Use exponential smoothing for baseline
        var alpha = 0.3m;
        var baseline = data.First().Amount;

        foreach (var record in data.Skip(1))
        {
            baseline = alpha * record.Amount + (1 - alpha) * baseline;
        }

        return baseline;
    }

    private decimal CalculateSeasonalFactor(DateTime date)
    {
        // Day of week seasonality
        var dayOfWeekFactor = date.DayOfWeek switch
        {
            DayOfWeek.Monday => 1.1m,
            DayOfWeek.Tuesday => 1.15m,
            DayOfWeek.Wednesday => 1.2m,
            DayOfWeek.Thursday => 1.15m,
            DayOfWeek.Friday => 1.25m,
            DayOfWeek.Saturday => 0.8m,
            DayOfWeek.Sunday => 0.6m,
            _ => 1.0m
        };

        // Monthly seasonality
        var monthFactor = date.Month switch
        {
            12 or 1 => 1.3m,    // Holiday season
            3 or 4 => 1.1m,     // Spring
            6 or 7 or 8 => 0.9m, // Summer slowdown
            9 or 10 => 1.15m,   // Fall pickup
            _ => 1.0m
        };

        return dayOfWeekFactor * monthFactor;
    }

    private decimal CalculateTrendFactor(List<Core.Entities.SalesRecord> data, int daysAhead)
    {
        if (data.Count < 10) return 1.0m;

        var recentPeriod = data.TakeLast(30).ToList();
        var previousPeriod = data.Skip(Math.Max(0, data.Count - 60)).Take(30).ToList();

        if (!previousPeriod.Any()) return 1.0m;

        var recentAvg = recentPeriod.Average(s => s.Amount);
        var previousAvg = previousPeriod.Average(s => s.Amount);

        var growthRate = previousAvg > 0 ? (recentAvg - previousAvg) / previousAvg : 0;
        var dailyGrowth = growthRate / 30.0m;

        // Apply diminishing returns for longer forecasts
        var adjustedGrowth = dailyGrowth / (1 + Math.Abs(dailyGrowth) * daysAhead * 0.1m);

        return Math.Max(0.5m, Math.Min(2.0m, 1 + (adjustedGrowth * daysAhead)));
    }

    private decimal CalculateVolatility(List<Core.Entities.SalesRecord> data)
    {
        if (data.Count < 2) return 0.2m;

        var amounts = data.Select(s => (double)s.Amount).ToList();
        var mean = amounts.Average();
        var variance = amounts.Sum(x => Math.Pow(x - mean, 2)) / amounts.Count;
        var stdDev = Math.Sqrt(variance);

        return (decimal)Math.Min(0.5, Math.Max(0.1, stdDev / mean));
    }
}

/// <summary>
/// Advanced CLV calculator using statistical models
/// </summary>
internal sealed class CLVCalculator
{
    private readonly List<Core.Entities.SalesRecord> _customerSales;
    private readonly ILogger _logger;

    public CLVCalculator(List<Core.Entities.SalesRecord> customerSales, ILogger logger)
    {
        _customerSales = customerSales;
        _logger = logger;
    }

    public decimal CalculateAdvancedCLV()
    {
        if (!_customerSales.Any()) return 0;

        var metrics = CalculateCustomerMetrics();
        var behaviorScore = CalculateBehaviorScore();
        var retentionProbability = CalculateRetentionProbability();
        var growthFactor = CalculateGrowthPotential();

        var baseCLV = metrics.AverageOrderValue * metrics.OrderFrequency * metrics.EstimatedLifetime;
        var adjustedCLV = baseCLV * retentionProbability * behaviorScore * growthFactor;

        return Math.Max(0, adjustedCLV);
    }

    private (decimal AverageOrderValue, decimal OrderFrequency, decimal EstimatedLifetime) CalculateCustomerMetrics()
    {
        var aov = _customerSales.Average(s => s.Amount);
        var totalDays = (DateTime.Now - _customerSales.First().Date).Days;
        var frequency = totalDays > 0 ? _customerSales.Count / (decimal)totalDays * 30 : 0;
        
        var lifetime = EstimateCustomerLifetime();
        
        return (aov, frequency, lifetime);
    }

    private decimal EstimateCustomerLifetime()
    {
        if (_customerSales.Count < 2) return 12m;

        var intervals = new List<double>();
        for (int i = 1; i < _customerSales.Count; i++)
        {
            intervals.Add((_customerSales[i].Date - _customerSales[i - 1].Date).TotalDays);
        }

        var avgInterval = intervals.Average();
        var recency = (DateTime.Now - _customerSales.Last().Date).TotalDays;

        // Customer lifecycle stage analysis
        if (recency > avgInterval * 4) return 3m;  // Likely churned
        if (recency > avgInterval * 2) return 6m;  // At risk
        if (_customerSales.Count > 10) return 36m; // Loyal customer
        if (_customerSales.Count > 5) return 24m;  // Regular customer
        
        return 12m; // New customer
    }

    private decimal CalculateBehaviorScore()
    {
        var consistencyScore = CalculateOrderConsistency();
        var valueScore = CalculateValueTrend();
        var recencyScore = CalculateRecencyScore();

        return (consistencyScore + valueScore + recencyScore) / 3m;
    }

    private decimal CalculateOrderConsistency()
    {
        if (_customerSales.Count < 3) return 0.5m;

        var intervals = new List<double>();
        for (int i = 1; i < _customerSales.Count; i++)
        {
            intervals.Add((_customerSales[i].Date - _customerSales[i - 1].Date).TotalDays);
        }

        var mean = intervals.Average();
        var variance = intervals.Sum(x => Math.Pow(x - mean, 2)) / intervals.Count;
        var coefficientOfVariation = Math.Sqrt(variance) / mean;

        return (decimal)Math.Max(0.1, Math.Min(1.0, 1.0 - (decimal)coefficientOfVariation / 2.0m));
    }

    private decimal CalculateValueTrend()
    {
        if (_customerSales.Count < 3) return 1.0m;

        var recentOrders = _customerSales.TakeLast(Math.Min(5, _customerSales.Count)).ToList();
        var earlyOrders = _customerSales.Take(Math.Min(5, _customerSales.Count)).ToList();

        var recentAvg = recentOrders.Average(s => s.Amount);
        var earlyAvg = earlyOrders.Average(s => s.Amount);

        var growthRate = earlyAvg > 0 ? (recentAvg - earlyAvg) / earlyAvg : 0;
        
        return Math.Max(0.5m, Math.Min(2.0m, 1.0m + (decimal)growthRate));
    }

    private decimal CalculateRecencyScore()
    {
        var daysSinceLastOrder = (DateTime.Now - _customerSales.Last().Date).TotalDays;
        var avgOrderInterval = _customerSales.Count > 1 
            ? (_customerSales.Last().Date - _customerSales.First().Date).TotalDays / (_customerSales.Count - 1)
            : 30;

        var recencyRatio = daysSinceLastOrder / avgOrderInterval;

        return (decimal)Math.Max(0.1, Math.Min(1.0, Math.Exp(-recencyRatio / 2.0)));
    }

    private decimal CalculateRetentionProbability()
    {
        var loyaltyScore = Math.Min(1.0m, _customerSales.Count * 0.1m);
        var recencyScore = CalculateRecencyScore();
        var consistencyScore = CalculateOrderConsistency();

        return (loyaltyScore + recencyScore + consistencyScore) / 3.0m;
    }

    private decimal CalculateGrowthPotential()
    {
        var categoryDiversity = _customerSales.Select(s => s.Category).Distinct().Count();
        var productDiversity = _customerSales.Select(s => s.ProductName).Distinct().Count();
        var orderGrowth = CalculateValueTrend();

        var diversityFactor = Math.Min(1.5m, 1.0m + (categoryDiversity * 0.1m) + ((decimal)productDiversity * 0.05m));
        
        return (diversityFactor + orderGrowth) / 2m;
    }
}

// ML Service DTOs
internal sealed record MLSalesData
{
    public string Date { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public int Quantity { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public string SalesRep { get; init; } = string.Empty;
    public decimal TotalValue { get; init; }
}

internal sealed record MLTrainingData
{
    public DateTime Date { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public int Quantity { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public string SalesRep { get; init; } = string.Empty;
    public string DayOfWeek { get; init; } = string.Empty;
    public int Month { get; init; }
    public int Quarter { get; init; }
    public bool IsWeekend { get; init; }
}

internal sealed record MLForecastRequest
{
    public List<MLSalesData> SalesData { get; init; } = new();
    public int ForecastDays { get; init; }
    public bool IncludeConfidenceIntervals { get; init; }
    public string ModelType { get; init; } = string.Empty;
}

internal sealed record MLForecastResponse
{
    public List<MLForecastItem> Forecasts { get; init; } = new();
    public double ModelAccuracy { get; init; }
    public string ModelVersion { get; init; } = string.Empty;
}

internal sealed record MLForecastItem
{
    public string Date { get; init; } = string.Empty;
    public decimal PredictedValue { get; init; }
    public decimal ConfidenceLevel { get; init; }
    public decimal LowerBound { get; init; }
    public decimal UpperBound { get; init; }
}

internal sealed record MLCLVRequest
{
    public string CustomerName { get; init; } = string.Empty;
    public List<MLSalesData> SalesHistory { get; init; } = new();
    public int PredictionHorizonMonths { get; init; }
}

internal sealed record MLCLVResponse
{
    public decimal PredictedCLV { get; init; }
    public decimal ConfidenceScore { get; init; }
    public string ModelVersion { get; init; } = string.Empty;
}

internal sealed record MLTrainingRequest
{
    public List<MLTrainingData> TrainingData { get; init; } = new();
    public string[] ModelTypes { get; init; } = Array.Empty<string>();
    public double ValidationSplit { get; init; }
    public int CrossValidationFolds { get; init; }
}

internal sealed record MLTrainingResponse
{
    public bool Success { get; init; }
    public double Accuracy { get; init; }
    public string ModelVersion { get; init; } = string.Empty;
    public Dictionary<string, double> Metrics { get; init; } = new();
}
