using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TrendLens.Application.Mappings;
using TrendLens.Application.Services;
using TrendLens.Application.Validators;
using TrendLens.Core.DTOs;
using TrendLens.Core.Entities;
using TrendLens.Core.Interfaces;
using Moq;
using Xunit;

namespace TrendLens.Tests.Unit.Services;

public class SalesServiceTests
{
    private readonly Mock<ISalesRepository> _mockRepository;
    private readonly Mock<ICsvImportService> _mockCsvService;
    private readonly Mock<ILogger<SalesService>> _mockLogger;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateSalesRecordDto> _createValidator;
    private readonly IValidator<UpdateSalesRecordDto> _updateValidator;
    private readonly SalesService _service;

    public SalesServiceTests()
    {
        _mockRepository = new Mock<ISalesRepository>();
        _mockCsvService = new Mock<ICsvImportService>();
        _mockLogger = new Mock<ILogger<SalesService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<SalesProfile>());
        _mapper = config.CreateMapper();

        _createValidator = new CreateSalesRecordValidator();
        _updateValidator = new UpdateSalesRecordValidator();

        _service = new SalesService(
            _mockRepository.Object,
            _mockCsvService.Object,
            _mapper,
            _createValidator,
            _updateValidator,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllSalesAsync_ReturnsSuccessResult()
    {
        var salesRecords = new List<SalesRecord>
        {
            new() { Id = 1, ProductName = "Test Product", Amount = 100m, Quantity = 1, Date = DateTime.Now, Category = "Test", CustomerName = "Test Customer", Region = "Test Region", SalesRep = "Test Rep" }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(salesRecords);

        var result = await _service.GetAllSalesAsync();

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task CreateSalesAsync_WithValidData_ReturnsSuccess()
    {
        var createDto = new CreateSalesRecordDto
        {
            Date = DateTime.Now.AddDays(-1),
            ProductName = "Test Product",
            Category = "Electronics",
            Amount = 100m,
            Quantity = 1,
            CustomerName = "Test Customer",
            Region = "North",
            SalesRep = "Test Rep"
        };

        var salesRecord = _mapper.Map<SalesRecord>(createDto);
        salesRecord.Id = 1;

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<SalesRecord>()))
            .ReturnsAsync(salesRecord);

        var result = await _service.CreateSalesAsync(createDto);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
    }

    [Fact]
    public async Task CreateSalesAsync_WithInvalidData_ReturnsFailure()
    {
        var createDto = new CreateSalesRecordDto
        {
            Date = DateTime.Now.AddDays(1), // Future date - invalid
            ProductName = "",
            Amount = -100m, // Negative amount - invalid
            Quantity = 0 // Zero quantity - invalid
        };

        var result = await _service.CreateSalesAsync(createDto);

        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task DeleteSalesAsync_WithValidId_ReturnsSuccess()
    {
        var salesRecord = new SalesRecord { Id = 1, ProductName = "Test" };

        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(salesRecord);

        _mockRepository.Setup(r => r.DeleteAsync(1))
            .Returns(Task.CompletedTask);

        var result = await _service.DeleteSalesAsync(1);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteSalesAsync_WithInvalidId_ReturnsFailure()
    {
        var result = await _service.DeleteSalesAsync(0);

        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid", result.Error);
    }
}
