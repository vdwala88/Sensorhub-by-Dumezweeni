using Microsoft.EntityFrameworkCore;
using SensorHub.Application.Interfaces;
using SensorHub.Domain.Entities;

namespace SensorHub.Infrastructure.Persistence;

public class SensorHubDbContext : DbContext
{
    private readonly ICurrentTenantAccessor _tenantAccessor;

    public SensorHubDbContext(DbContextOptions<SensorHubDbContext> options, ICurrentTenantAccessor tenantAccessor)
        : base(options)
    {
        _tenantAccessor = tenantAccessor;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Sensor> Sensors => Set<Sensor>();
    public DbSet<SensorReading> Readings => Set<SensorReading>();
    public DbSet<AlertRule> AlertRules => Set<AlertRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>().HasKey(t => t.Id);

        modelBuilder.Entity<Sensor>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasOne<Tenant>().WithMany(t => t.Sensors).HasForeignKey(s => s.TenantId);
            entity.HasMany(s => s.Readings).WithOne(r => r.Sensor!).HasForeignKey(r => r.SensorId);
            entity.HasMany(s => s.AlertRules).WithOne(r => r.Sensor!).HasForeignKey(r => r.SensorId);

            entity.HasQueryFilter(s => _tenantAccessor.TenantId == null || s.TenantId == _tenantAccessor.TenantId);
        });

        modelBuilder.Entity<SensorReading>().HasKey(r => r.Id);
        modelBuilder.Entity<AlertRule>().HasKey(r => r.Id);
    }
}
