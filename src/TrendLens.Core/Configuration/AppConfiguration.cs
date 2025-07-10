using System.ComponentModel.DataAnnotations;

namespace TrendLens.Core.Configuration;

/// <summary>
/// Configuration settings for the Machine Learning service
/// </summary>
public sealed class MLServiceConfiguration
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "MLService";

    /// <summary>
    /// Base URL of the ML service
    /// </summary>
    [Required]
    [Url]
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// API key for authentication
    /// </summary>
    [Required]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    [Range(1, 300)]
    public int Timeout { get; set; } = 30;

    /// <summary>
    /// Number of retry attempts
    /// </summary>
    [Range(0, 10)]
    public int RetryAttempts { get; set; } = 3;

    /// <summary>
    /// Delay between retries in milliseconds
    /// </summary>
    [Range(100, 10000)]
    public int RetryDelay { get; set; } = 1000;

    /// <summary>
    /// Enable circuit breaker pattern
    /// </summary>
    public bool EnableCircuitBreaker { get; set; } = true;

    /// <summary>
    /// Circuit breaker failure threshold
    /// </summary>
    [Range(1, 20)]
    public int CircuitBreakerThreshold { get; set; } = 5;

    /// <summary>
    /// Circuit breaker duration in milliseconds
    /// </summary>
    [Range(1000, 300000)]
    public int CircuitBreakerDuration { get; set; } = 60000;
}

/// <summary>
/// Feature toggle configuration
/// </summary>
public sealed class FeaturesConfiguration
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "Features";

    /// <summary>
    /// Enable ML service integration
    /// </summary>
    public bool EnableMLService { get; set; } = true;

    /// <summary>
    /// Enable advanced analytics features
    /// </summary>
    public bool EnableAdvancedAnalytics { get; set; } = true;

    /// <summary>
    /// Enable caching
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// Enable rate limiting
    /// </summary>
    public bool EnableRateLimiting { get; set; } = true;

    /// <summary>
    /// Enable health checks
    /// </summary>
    public bool EnableHealthChecks { get; set; } = true;

    /// <summary>
    /// Enable metrics collection
    /// </summary>
    public bool EnableMetrics { get; set; } = true;

    /// <summary>
    /// Maximum CSV file size in bytes
    /// </summary>
    [Range(1024, 1073741824)] // 1KB to 1GB
    public long MaxCsvFileSize { get; set; } = 52428800; // 50MB

    /// <summary>
    /// Maximum number of records per CSV file
    /// </summary>
    [Range(1, 1000000)]
    public int MaxCsvRecords { get; set; } = 100000;

    /// <summary>
    /// Enable Swagger documentation
    /// </summary>
    public bool EnableSwagger { get; set; } = true;

    /// <summary>
    /// Enable CORS
    /// </summary>
    public bool EnableCors { get; set; } = true;
}

/// <summary>
/// Caching configuration
/// </summary>
public sealed class CachingConfiguration
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "Caching";

    /// <summary>
    /// Default cache expiration time
    /// </summary>
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Sliding expiration time
    /// </summary>
    public TimeSpan SlidingExpiration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Analytics cache expiration time
    /// </summary>
    public TimeSpan AnalyticsExpiration { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Forecast cache expiration time
    /// </summary>
    public TimeSpan ForecastExpiration { get; set; } = TimeSpan.FromHours(1);
}

/// <summary>
/// Rate limiting configuration
/// </summary>
public sealed class RateLimitingConfiguration
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "RateLimiting";

    /// <summary>
    /// Default rate limiting policy
    /// </summary>
    public RateLimitPolicy DefaultPolicy { get; set; } = new();

    /// <summary>
    /// API rate limiting policy
    /// </summary>
    public RateLimitPolicy ApiPolicy { get; set; } = new();
}

/// <summary>
/// Rate limit policy configuration
/// </summary>
public sealed class RateLimitPolicy
{
    /// <summary>
    /// Maximum number of permits
    /// </summary>
    [Range(1, int.MaxValue)]
    public int PermitLimit { get; set; } = 100;

    /// <summary>
    /// Time window for rate limiting
    /// </summary>
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Token replenishment period
    /// </summary>
    public TimeSpan ReplenishmentPeriod { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Tokens per replenishment period
    /// </summary>
    [Range(1, int.MaxValue)]
    public int TokensPerPeriod { get; set; } = 10;

    /// <summary>
    /// Queue processing order
    /// </summary>
    public string QueueProcessingOrder { get; set; } = "OldestFirst";

    /// <summary>
    /// Maximum queue size
    /// </summary>
    [Range(1, int.MaxValue)]
    public int QueueLimit { get; set; } = 50;
}

/// <summary>
/// CORS configuration
/// </summary>
public sealed class CorsConfiguration
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "Cors";

    /// <summary>
    /// Allowed origins
    /// </summary>
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Allowed HTTP methods
    /// </summary>
    public string[] AllowedMethods { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Allowed headers
    /// </summary>
    public string[] AllowedHeaders { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Allow credentials
    /// </summary>
    public bool AllowCredentials { get; set; } = true;

    /// <summary>
    /// Preflight max age in seconds
    /// </summary>
    [Range(0, 86400)] // 0 to 24 hours
    public int MaxAge { get; set; } = 3600;
}

/// <summary>
/// Security configuration
/// </summary>
public sealed class SecurityConfiguration
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "Security";

    /// <summary>
    /// Require HTTPS
    /// </summary>
    public bool RequireHttps { get; set; } = false;

    /// <summary>
    /// Enable API key authentication
    /// </summary>
    public bool EnableApiKey { get; set; } = false;

    /// <summary>
    /// API key header name
    /// </summary>
    public string ApiKeyHeader { get; set; } = "X-API-Key";

    /// <summary>
    /// Enable JWT authentication
    /// </summary>
    public bool EnableJwt { get; set; } = false;

    /// <summary>
    /// JWT secret key
    /// </summary>
    public string JwtSecret { get; set; } = string.Empty;

    /// <summary>
    /// JWT token expiration time
    /// </summary>
    public TimeSpan JwtExpiration { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// JWT issuer
    /// </summary>
    public string JwtIssuer { get; set; } = string.Empty;

    /// <summary>
    /// JWT audience
    /// </summary>
    public string JwtAudience { get; set; } = string.Empty;
}

/// <summary>
/// Performance configuration
/// </summary>
public sealed class PerformanceConfiguration
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "Performance";

    /// <summary>
    /// Database command timeout in seconds
    /// </summary>
    [Range(1, 300)]
    public int DatabaseCommandTimeout { get; set; } = 30;

    /// <summary>
    /// HTTP client timeout in seconds
    /// </summary>
    [Range(1, 300)]
    public int HttpClientTimeout { get; set; } = 30;

    /// <summary>
    /// Maximum concurrent requests
    /// </summary>
    [Range(1, 10000)]
    public int MaxConcurrentRequests { get; set; } = 100;

    /// <summary>
    /// Enable response compression
    /// </summary>
    public bool EnableResponseCompression { get; set; } = true;

    /// <summary>
    /// Enable output caching
    /// </summary>
    public bool EnableOutputCaching { get; set; } = true;
}

/// <summary>
/// Analytics configuration
/// </summary>
public sealed class AnalyticsConfiguration
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "Analytics";

    /// <summary>
    /// Default date range in days
    /// </summary>
    [Range(1, 365)]
    public int DefaultDateRange { get; set; } = 30;

    /// <summary>
    /// Maximum allowed date range in days
    /// </summary>
    [Range(1, 3650)]
    public int MaxDateRange { get; set; } = 365;

    /// <summary>
    /// Cache analytics results
    /// </summary>
    public bool CacheAnalytics { get; set; } = true;

    /// <summary>
    /// Enable real-time updates
    /// </summary>
    public bool EnableRealTimeUpdates { get; set; } = false;

    /// <summary>
    /// Batch size for processing
    /// </summary>
    [Range(100, 10000)]
    public int BatchSize { get; set; } = 1000;
}
