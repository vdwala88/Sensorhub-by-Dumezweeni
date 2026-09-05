using SensorHub.Application.Interfaces;
using SensorHub.Domain.Entities;

namespace SensorHub.Application.Services;

public class SensorService : ISensorService
{
    private readonly ISensorRepository _repository;
    private readonly IAlertEngine _alertEngine;

    public SensorService(ISensorRepository repository, IAlertEngine alertEngine)
    {
        _repository = repository;
        _alertEngine = alertEngine;
    }

    public Task<IEnumerable<Sensor>> GetAllAsync() => _repository.GetAllAsync();

    public Task<Sensor?> GetByIdAsync(Guid sensorId) => _repository.GetByIdAsync(sensorId);

    public async Task<Sensor> RegisterAsync(Sensor sensor)
    {
        if (string.IsNullOrWhiteSpace(sensor.Name))
            throw new ArgumentException("Sensor name is required.", nameof(sensor));

        sensor.Id = sensor.Id == Guid.Empty ? Guid.NewGuid() : sensor.Id;
        sensor.CreatedAt = DateTime.UtcNow;
        return await _repository.AddAsync(sensor);
    }

    public async Task RecordReadingAsync(Guid sensorId, double value, DateTime timestamp)
    {
        var sensor = await _repository.GetByIdAsync(sensorId)
            ?? throw new InvalidOperationException($"Sensor {sensorId} was not found.");

        var reading = new SensorReading
        {
            Id = Guid.NewGuid(),
            SensorId = sensor.Id,
            Value = value,
            Timestamp = timestamp
        };

        await _repository.AddReadingAsync(reading);
        await _alertEngine.EvaluateAsync(sensor.Id, value);
    }
}
