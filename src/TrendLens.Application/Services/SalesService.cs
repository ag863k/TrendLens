using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TrendLens.Core.Common;
using TrendLens.Core.DTOs;
using TrendLens.Core.Entities;
using TrendLens.Core.Interfaces;

namespace TrendLens.Application.Services;

public class SalesService : ISalesService
{
    private readonly ISalesRepository _salesRepository;
    private readonly ICsvImportService _csvImportService;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateSalesRecordDto> _createValidator;
    private readonly IValidator<UpdateSalesRecordDto> _updateValidator;
    private readonly ILogger<SalesService> _logger;

    public SalesService(
        ISalesRepository salesRepository,
        ICsvImportService csvImportService,
        IMapper mapper,
        IValidator<CreateSalesRecordDto> createValidator,
        IValidator<UpdateSalesRecordDto> updateValidator,
        ILogger<SalesService> logger)
    {
        _salesRepository = salesRepository;
        _csvImportService = csvImportService;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<SalesRecordDto>>> GetAllSalesAsync()
    {
        try
        {
            var sales = await _salesRepository.GetAllAsync();
            var salesDtos = _mapper.Map<IEnumerable<SalesRecordDto>>(sales);
            return Result<IEnumerable<SalesRecordDto>>.Success(salesDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all sales records");
            return Result<IEnumerable<SalesRecordDto>>.Failure("Failed to retrieve sales records");
        }
    }

    public async Task<Result<SalesRecordDto>> GetSalesByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
                return Result<SalesRecordDto>.Failure("Invalid sales record ID");

            var sales = await _salesRepository.GetByIdAsync(id);
            if (sales == null)
                return Result<SalesRecordDto>.Failure($"Sales record with ID {id} not found");

            var salesDto = _mapper.Map<SalesRecordDto>(sales);
            return Result<SalesRecordDto>.Success(salesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sales record with ID {Id}", id);
            return Result<SalesRecordDto>.Failure("Failed to retrieve sales record");
        }
    }

    public async Task<Result<SalesRecordDto>> CreateSalesAsync(CreateSalesRecordDto createDto)
    {
        try
        {
            var validationResult = await _createValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<SalesRecordDto>.Failure(errors);
            }

            var salesRecord = _mapper.Map<SalesRecord>(createDto);
            var created = await _salesRepository.AddAsync(salesRecord);
            var createdDto = _mapper.Map<SalesRecordDto>(created);

            _logger.LogInformation("Successfully created sales record with ID {Id}", created.Id);
            return Result<SalesRecordDto>.Success(createdDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sales record");
            return Result<SalesRecordDto>.Failure("Failed to create sales record");
        }
    }

    public async Task<Result<SalesRecordDto>> UpdateSalesAsync(UpdateSalesRecordDto updateDto)
    {
        try
        {
            var validationResult = await _updateValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<SalesRecordDto>.Failure(errors);
            }

            var existing = await _salesRepository.GetByIdAsync(updateDto.Id);
            if (existing == null)
                return Result<SalesRecordDto>.Failure($"Sales record with ID {updateDto.Id} not found");

            var salesRecord = _mapper.Map<SalesRecord>(updateDto);
            await _salesRepository.UpdateAsync(salesRecord);
            var updatedDto = _mapper.Map<SalesRecordDto>(salesRecord);

            _logger.LogInformation("Successfully updated sales record with ID {Id}", updateDto.Id);
            return Result<SalesRecordDto>.Success(updatedDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sales record with ID {Id}", updateDto.Id);
            return Result<SalesRecordDto>.Failure("Failed to update sales record");
        }
    }

    public async Task<Result> DeleteSalesAsync(int id)
    {
        try
        {
            if (id <= 0)
                return Result.Failure("Invalid sales record ID");

            var existing = await _salesRepository.GetByIdAsync(id);
            if (existing == null)
                return Result.Failure($"Sales record with ID {id} not found");

            await _salesRepository.DeleteAsync(id);
            _logger.LogInformation("Successfully deleted sales record with ID {Id}", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting sales record with ID {Id}", id);
            return Result.Failure("Failed to delete sales record");
        }
    }

    public async Task<Result<int>> ImportCsvAsync(Stream csvStream, string fileName)
    {
        try
        {
            if (csvStream == null || csvStream.Length == 0)
                return Result<int>.Failure("No file provided");

            if (!_csvImportService.ValidateCsvFormat(csvStream))
                return Result<int>.Failure("Invalid CSV format");

            csvStream.Position = 0;
            var salesRecords = await _csvImportService.ImportFromCsvAsync(csvStream);
            var importedRecords = await _salesRepository.AddRangeAsync(salesRecords);

            var count = importedRecords.Count();
            _logger.LogInformation("Successfully imported {Count} records from file {FileName}", count, fileName);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing CSV file {FileName}", fileName);
            return Result<int>.Failure("Failed to import CSV file");
        }
    }

    public async Task<Result<PaginatedResultDto<SalesRecordDto>>> SearchSalesAsync(SalesRecordSearchDto searchDto)
    {
        try
        {
            _logger.LogInformation("Searching sales records with criteria");
            
            var sales = await _salesRepository.GetAllAsync();
            var query = sales.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchDto.CustomerName))
            {
                query = query.Where(s => s.CustomerName.Contains(searchDto.CustomerName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(searchDto.ProductName))
            {
                query = query.Where(s => s.ProductName.Contains(searchDto.ProductName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(searchDto.Category))
            {
                query = query.Where(s => s.Category.Contains(searchDto.Category, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(searchDto.Region))
            {
                query = query.Where(s => s.Region.Contains(searchDto.Region, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(searchDto.SalesRep))
            {
                query = query.Where(s => s.SalesRep.Contains(searchDto.SalesRep, StringComparison.OrdinalIgnoreCase));
            }

            if (searchDto.StartDate.HasValue)
            {
                query = query.Where(s => s.Date >= searchDto.StartDate.Value);
            }

            if (searchDto.EndDate.HasValue)
            {
                query = query.Where(s => s.Date <= searchDto.EndDate.Value);
            }

            if (searchDto.MinAmount.HasValue)
            {
                query = query.Where(s => s.Amount >= searchDto.MinAmount.Value);
            }

            if (searchDto.MaxAmount.HasValue)
            {
                query = query.Where(s => s.Amount <= searchDto.MaxAmount.Value);
            }

            // Apply sorting
            query = searchDto.SortBy?.ToLower() switch
            {
                "date" => searchDto.SortDirection == "desc" ? query.OrderByDescending(s => s.Date) : query.OrderBy(s => s.Date),
                "amount" => searchDto.SortDirection == "desc" ? query.OrderByDescending(s => s.Amount) : query.OrderBy(s => s.Amount),
                "customer" => searchDto.SortDirection == "desc" ? query.OrderByDescending(s => s.CustomerName) : query.OrderBy(s => s.CustomerName),
                "product" => searchDto.SortDirection == "desc" ? query.OrderByDescending(s => s.ProductName) : query.OrderBy(s => s.ProductName),
                _ => query.OrderByDescending(s => s.Date)
            };

            var totalCount = query.Count();
            var pagedResults = query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToList();

            var salesDtos = _mapper.Map<IEnumerable<SalesRecordDto>>(pagedResults);

            var result = new PaginatedResultDto<SalesRecordDto>
            {
                Items = salesDtos,
                PageNumber = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalCount = totalCount
            };

            return Result<PaginatedResultDto<SalesRecordDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching sales records");
            return Result<PaginatedResultDto<SalesRecordDto>>.Failure("Failed to search sales records");
        }
    }

    public async Task<Result<IEnumerable<SalesRecordDto>>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation("Getting sales records between {StartDate} and {EndDate}", startDate, endDate);
            
            if (startDate > endDate)
            {
                return Result<IEnumerable<SalesRecordDto>>.Failure("Start date cannot be greater than end date");
            }

            var sales = await _salesRepository.GetByDateRangeAsync(startDate, endDate);
            var salesDtos = _mapper.Map<IEnumerable<SalesRecordDto>>(sales);

            _logger.LogInformation("Found {Count} sales records in date range", salesDtos.Count());
            return Result<IEnumerable<SalesRecordDto>>.Success(salesDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sales by date range");
            return Result<IEnumerable<SalesRecordDto>>.Failure("Failed to get sales by date range");
        }
    }
}
