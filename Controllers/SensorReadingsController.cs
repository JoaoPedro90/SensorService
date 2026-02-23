using Microsoft.AspNetCore.Mvc;
using SensorService.Application.Dtos;
using SensorService.Application.Services;
using SensorService.Domain.Exceptions;

namespace SensorService.Controllers;

[ApiController]
[Route("api/talhoes/{talhaoId:guid}/leituras")]
public class SensorReadingsController : ControllerBase
{
    private readonly SensorReadingAppService _service;

    public SensorReadingsController(SensorReadingAppService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromRoute] Guid talhaoId, [FromBody] CreateSensorReadingRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _service.IngestAsync(talhaoId, request, ct);
            return CreatedAtAction(nameof(GetLatest), new { talhaoId }, result);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetLatest([FromRoute] Guid talhaoId, [FromQuery] int limit = 50, CancellationToken ct = default)
    {
        var result = await _service.GetLatestAsync(talhaoId, limit, ct);
        return Ok(result);
    }
}