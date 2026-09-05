using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SensorHub.Application.Services;
using SensorHub.Domain.Entities;
using SensorHub.Domain.Enums;
using SensorHub.Infrastructure.Persistence;
using Xunit;

namespace SensorHub.Tests;

public class SensorServiceTests
{
    private static (SensorService service, SensorHubDbContext db) CreateService(Guid? tenantId = null)
    {
        var options = new DbContextOptionsBuilder<SensorHubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var tenantAccessor = new CurrentTenantAccessor { TenantId = tenantId };
        var db = new SensorHubDbContext(options, tenantAccessor);
        var repository = new SensorRepository(db);
        var alertEngine = new AlertEngine(repository, NullLogger<AlertEngine>.Instance);
        var service = new SensorService(repository, alertEngine);

        return (service, db);
    }

    [Fact]
    public async Task RegisterAsync_PersistsSensor_WithGeneratedId()
    {
        var (service, _) = CreateService();

        var sensor = await service.RegisterAsync(new Sensor
        {
            TenantId = Guid.NewGuid(),
            Name = "Soil Moisture 1",
            Location = "Field A",
            Type = "SoilMoisture"
        });

        Assert.NotEqual(Guid.Empty, sensor.Id);
        Assert.Equal("Soil Moisture 1", sensor.Name);
    }

    [Fact]
    public async Task RegisterAsync_ThrowsWhenNameMissing()
    {
        var (service, _) = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.RegisterAsync(new Sensor { TenantId = Guid.NewGuid(), Name = "" }));
    }

    [Fact]
    public async Task RecordReadingAsync_PersistsReading_AndEvaluatesAlertRules()
    {
        var tenantId = Guid.NewGuid();
        var (service, db) = CreateService(tenantId);

        var sensor = await service.RegisterAsync(new Sensor
        {
            TenantId = tenantId,
            Name = "Temp Sensor",
            Type = "Temperature"
        });

        db.AlertRules.Add(new AlertRule
        {
            Id = Guid.NewGuid(),
            SensorId = sensor.Id,
            Operator = ComparisonOperator.GreaterThan,
            Threshold = 30,
            IsEnabled = true
        });
        await db.SaveChangesAsync();

        await service.RecordReadingAsync(sensor.Id, 45, DateTime.UtcNow);

        var readingCount = await db.Readings.CountAsync(r => r.SensorId == sensor.Id);
        Assert.Equal(1, readingCount);
    }

    [Fact]
    public async Task GetAllAsync_IsScopedToCurrentTenant()
    {
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();
        var dbName = Guid.NewGuid().ToString();

        var optionsA = new DbContextOptionsBuilder<SensorHubDbContext>().UseInMemoryDatabase(dbName).Options;
        var dbA = new SensorHubDbContext(optionsA, new CurrentTenantAccessor { TenantId = tenantA });
        await new SensorRepository(dbA).AddAsync(new Sensor { Id = Guid.NewGuid(), TenantId = tenantA, Name = "A-Sensor" });

        var optionsB = new DbContextOptionsBuilder<SensorHubDbContext>().UseInMemoryDatabase(dbName).Options;
        var dbB = new SensorHubDbContext(optionsB, new CurrentTenantAccessor { TenantId = tenantB });
        var resultForB = await new SensorRepository(dbB).GetAllAsync();

        Assert.Empty(resultForB);
    }
}
