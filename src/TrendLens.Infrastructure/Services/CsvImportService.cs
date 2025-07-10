using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.Extensions.Logging;
using TrendLens.Core.Entities;
using TrendLens.Core.Interfaces;

namespace TrendLens.Infrastructure.Services;

public class CsvImportService : ICsvImportService
{
    private readonly ILogger<CsvImportService> _logger;

    public CsvImportService(ILogger<CsvImportService> logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<SalesRecord>> ImportFromCsvAsync(Stream csvStream)
    {
        try
        {
            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            
            csv.Context.RegisterClassMap<SalesRecordMap>();
            
            var records = new List<SalesRecord>();
            var lineNumber = 1;
            
            await foreach (var record in csv.GetRecordsAsync<SalesRecord>())
            {
                lineNumber++;
                
                if (IsValidRecord(record, lineNumber))
                {
                    records.Add(record);
                }
            }
            
            _logger.LogInformation("Successfully imported {Count} records from CSV", records.Count);
            return records;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing CSV data");
            throw new InvalidOperationException("Failed to import CSV data", ex);
        }
    }

    public bool ValidateCsvFormat(Stream csvStream)
    {
        try
        {
            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            
            csv.Read();
            csv.ReadHeader();
            
            var requiredHeaders = new[] { "Date", "ProductName", "Category", "Amount", "Quantity", "CustomerName", "Region", "SalesRep" };
            var hasAllHeaders = requiredHeaders.All(header => 
                csv.HeaderRecord?.Contains(header, StringComparer.OrdinalIgnoreCase) == true);
            
            if (!hasAllHeaders)
            {
                _logger.LogWarning("CSV validation failed. Missing required headers");
                return false;
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating CSV format");
            return false;
        }
    }

    private bool IsValidRecord(SalesRecord record, int lineNumber)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(record.ProductName))
            errors.Add("ProductName is required");

        if (string.IsNullOrWhiteSpace(record.Category))
            errors.Add("Category is required");

        if (record.Amount <= 0)
            errors.Add("Amount must be greater than 0");

        if (record.Quantity <= 0)
            errors.Add("Quantity must be greater than 0");

        if (string.IsNullOrWhiteSpace(record.CustomerName))
            errors.Add("CustomerName is required");

        if (string.IsNullOrWhiteSpace(record.Region))
            errors.Add("Region is required");

        if (string.IsNullOrWhiteSpace(record.SalesRep))
            errors.Add("SalesRep is required");

        if (errors.Any())
        {
            _logger.LogWarning("Invalid record at line {LineNumber}: {Errors}", 
                lineNumber, string.Join(", ", errors));
            return false;
        }

        return true;
    }
}

public class SalesRecordMap : ClassMap<SalesRecord>
{
    public SalesRecordMap()
    {
        Map(m => m.Date).Name("Date").TypeConverter<DateTimeConverter>();
        Map(m => m.ProductName).Name("ProductName");
        Map(m => m.Category).Name("Category");
        Map(m => m.Amount).Name("Amount").TypeConverter<DecimalConverter>();
        Map(m => m.Quantity).Name("Quantity").TypeConverter<Int32Converter>();
        Map(m => m.CustomerName).Name("CustomerName");
        Map(m => m.Region).Name("Region");
        Map(m => m.SalesRep).Name("SalesRep");
    }
}
