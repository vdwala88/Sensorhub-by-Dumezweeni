using Microsoft.EntityFrameworkCore;
using SensorHub.Application.Interfaces;
using SensorHub.Domain.Entities;

namespace SensorHub.Infrastructure.Persistence;

public class SensorRepository : ISensorRepository
{
    private readonly SensorHubDbContext _db;

    public SensorRepository(SensorHubDbContext db) => _db = db;

    public async Task<IEnumerable<Sensor>> GetAllAsync(CancellationToken ct = default)
        => await _db.Sensors.AsNoTracking().ToListAsync(ct);

    public async Task<Sensor?> GetByIdAsync(Guid sensorId, CancellationToken ct = default)
        => await _db.Sensors.AsNoTracking().FirstOrDefaultAsync(s => s.Id == sensorId, ct);

    public async Task<Sensor> AddAsync(Sensor sensor, CancellationToken ct = default)
    {
        _db.Sensors.Add(sensor);
        await _db.SaveChangesAsync(ct);
        return sensor;
    }

    public async Task AddReadingAsync(SensorReading reading, CancellationToken ct = default)
    {
        _db.Readings.Add(reading);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<AlertRule>> GetActiveRulesForSensorAsync(Guid sensorId, CancellationToken ct = default)
        => await _db.AlertRules.AsNoTracking()
            .Where(r => r.SensorId == sensorId && r.IsEnabled)
            .ToListAsync(ct);

    public async Task<SensorReading?> GetLatestReadingAsync(Guid sensorId, CancellationToken ct = default)
        => await _db.Readings.AsNoTracking()
            .Where(r => r.SensorId == sensorId)
            .OrderByDescending(r => r.Timestamp)
            .FirstOrDefaultAsync(ct);
}
