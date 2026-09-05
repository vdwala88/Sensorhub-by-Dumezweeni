using SensorHub.Domain.Entities;

namespace SensorHub.Application.Interfaces;

public interface ISensorService
{
    Task<IEnumerable<Sensor>> GetAllAsync();
    Task<Sensor?> GetByIdAsync(Guid sensorId);
    Task<Sensor> RegisterAsync(Sensor sensor);
    Task RecordReadingAsync(Guid sensorId, double value, DateTime timestamp);
}
