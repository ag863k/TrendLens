using Microsoft.AspNetCore.Mvc;
using TrendLens.Core.DTOs;
using TrendLens.Core.Interfaces;

namespace TrendLens.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly ISalesService _salesService;
    private readonly ILogger<SalesController> _logger;

    public SalesController(ISalesService salesService, ILogger<SalesController> logger)
    {
        _salesService = salesService;
        _logger = logger;
    }

    /// <summary>
    /// Get all sales records
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SalesRecordDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IEnumerable<SalesRecordDto>>> GetSales()
    {
        var result = await _salesService.GetAllSalesAsync();
        
        if (!result.IsSuccess)
            return StatusCode(500, result.Error);
        
        return Ok(result.Data);
    }

    /// <summary>
    /// Get a specific sales record by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SalesRecordDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<SalesRecordDto>> GetSalesRecord(int id)
    {
        var result = await _salesService.GetSalesByIdAsync(id);
        
        if (!result.IsSuccess)
        {
            if (result.Error.Contains("not found"))
                return NotFound(result.Error);
            if (result.Error.Contains("Invalid"))
                return BadRequest(result.Error);
            return StatusCode(500, result.Error);
        }
        
        return Ok(result.Data);
    }

    /// <summary>
    /// Create a new sales record
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SalesRecordDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<SalesRecordDto>> CreateSalesRecord([FromBody] CreateSalesRecordDto createDto)
    {
        var result = await _salesService.CreateSalesAsync(createDto);
        
        if (!result.IsSuccess)
        {
            if (result.Errors.Any())
                return BadRequest(new { Errors = result.Errors });
            return StatusCode(500, result.Error);
        }
        
        return CreatedAtAction(nameof(GetSalesRecord), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Upload and import sales data from CSV file
    /// </summary>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> UploadCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        if (file.Length > 50 * 1024 * 1024) // 50MB limit
            return BadRequest("File size exceeds 50MB limit");

        using var stream = file.OpenReadStream();
        var result = await _salesService.ImportCsvAsync(stream, file.FileName);
        
        if (!result.IsSuccess)
            return BadRequest(result.Error);
        
        return Ok(new { Message = $"Successfully imported {result.Data} records", Count = result.Data });
    }

    /// <summary>
    /// Update an existing sales record
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(SalesRecordDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateSalesRecord(int id, [FromBody] UpdateSalesRecordDto updateDto)
    {
        if (id != updateDto.Id)
            return BadRequest("ID in URL does not match ID in request body");

        var result = await _salesService.UpdateSalesAsync(updateDto);
        
        if (!result.IsSuccess)
        {
            if (result.Error.Contains("not found"))
                return NotFound(result.Error);
            if (result.Errors.Any())
                return BadRequest(new { Errors = result.Errors });
            return StatusCode(500, result.Error);
        }
        
        return Ok(result.Data);
    }

    /// <summary>
    /// Delete a sales record
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteSalesRecord(int id)
    {
        var result = await _salesService.DeleteSalesAsync(id);
        
        if (!result.IsSuccess)
        {
            if (result.Error.Contains("not found"))
                return NotFound(result.Error);
            if (result.Error.Contains("Invalid"))
                return BadRequest(result.Error);
            return StatusCode(500, result.Error);
        }
        
        return NoContent();
    }
}
