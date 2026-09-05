using SensorHub.Domain.Entities;

namespace SensorHub.Application.Interfaces;

public interface ISensorRepository
{
    Task<IEnumerable<Sensor>> GetAllAsync(CancellationToken ct = default);
    Task<Sensor?> GetByIdAsync(Guid sensorId, CancellationToken ct = default);
    Task<Sensor> AddAsync(Sensor sensor, CancellationToken ct = default);
    Task AddReadingAsync(SensorReading reading, CancellationToken ct = default);
    Task<IEnumerable<AlertRule>> GetActiveRulesForSensorAsync(Guid sensorId, CancellationToken ct = default);
    Task<SensorReading?> GetLatestReadingAsync(Guid sensorId, CancellationToken ct = default);
}
