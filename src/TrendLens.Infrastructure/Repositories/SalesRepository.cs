using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrendLens.Core.Entities;
using TrendLens.Core.Interfaces;
using TrendLens.Infrastructure.Data;

namespace TrendLens.Infrastructure.Repositories;

public class SalesRepository : ISalesRepository
{
    private readonly TrendLensDbContext _context;
    private readonly ILogger<SalesRepository> _logger;

    public SalesRepository(TrendLensDbContext context, ILogger<SalesRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<SalesRecord>> GetAllAsync()
    {
        try
        {
            return await _context.SalesRecords
                .OrderByDescending(s => s.Date)
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all sales records");
            throw;
        }
    }

    public async Task<SalesRecord?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.SalesRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sales record with ID {Id}", id);
            throw;
        }
    }

    public async Task<SalesRecord> AddAsync(SalesRecord salesRecord)
    {
        try
        {
            _context.SalesRecords.Add(salesRecord);
            await _context.SaveChangesAsync();
            _logger.LogDebug("Added sales record with ID {Id}", salesRecord.Id);
            return salesRecord;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding sales record");
            throw;
        }
    }

    public async Task<IEnumerable<SalesRecord>> AddRangeAsync(IEnumerable<SalesRecord> salesRecords)
    {
        try
        {
            var recordsList = salesRecords.ToList();
            _context.SalesRecords.AddRange(recordsList);
            await _context.SaveChangesAsync();
            _logger.LogDebug("Added {Count} sales records", recordsList.Count);
            return recordsList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding range of sales records");
            throw;
        }
    }

    public async Task UpdateAsync(SalesRecord salesRecord)
    {
        try
        {
            _context.Entry(salesRecord).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _logger.LogDebug("Updated sales record with ID {Id}", salesRecord.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sales record with ID {Id}", salesRecord.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var salesRecord = await _context.SalesRecords.FindAsync(id);
            if (salesRecord != null)
            {
                _context.SalesRecords.Remove(salesRecord);
                await _context.SaveChangesAsync();
                _logger.LogDebug("Deleted sales record with ID {Id}", id);
            }
            else
            {
                _logger.LogWarning("Attempted to delete non-existent sales record with ID {Id}", id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting sales record with ID {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<SalesRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            return await _context.SalesRecords
                .Where(s => s.Date >= startDate && s.Date <= endDate)
                .OrderByDescending(s => s.Date)
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sales records for date range {StartDate} to {EndDate}", startDate, endDate);
            throw;
        }
    }
}
