using Microsoft.AspNetCore.Mvc;
using SensorHub.Application.Interfaces;
using SensorHub.Domain.Entities;
using SensorHub.Shared.Dtos;

namespace SensorHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorsController : ControllerBase
{
    private readonly ISensorService _sensorService;

    public SensorsController(ISensorService sensorService) => _sensorService = sensorService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SensorDto>>> GetAll()
    {
        var sensors = await _sensorService.GetAllAsync();
        return Ok(sensors.Select(ToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SensorDto>> GetById(Guid id)
    {
        var sensor = await _sensorService.GetByIdAsync(id);
        return sensor is null ? NotFound() : Ok(ToDto(sensor));
    }

    [HttpPost]
    public async Task<ActionResult<SensorDto>> Register([FromBody] RegisterSensorRequest request)
    {
        var sensor = new Sensor
        {
            TenantId = request.TenantId,
            Name = request.Name,
            Location = request.Location,
            Type = request.Type
        };

        var created = await _sensorService.RegisterAsync(sensor);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPost("{id:guid}/readings")]
    public async Task<IActionResult> RecordReading(Guid id, [FromBody] RecordReadingRequest request)
    {
        await _sensorService.RecordReadingAsync(id, request.Value, request.Timestamp ?? DateTime.UtcNow);
        return Accepted();
    }

    private static SensorDto ToDto(Sensor s) => new(s.Id, s.Name, s.Location, s.Type, s.IsActive);
}

public record RegisterSensorRequest(Guid TenantId, string Name, string Location, string Type);
public record RecordReadingRequest(double Value, DateTime? Timestamp);
