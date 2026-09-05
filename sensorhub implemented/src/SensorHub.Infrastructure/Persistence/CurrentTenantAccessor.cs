using SensorHub.Application.Interfaces;

namespace SensorHub.Infrastructure.Persistence;

public class CurrentTenantAccessor : ICurrentTenantAccessor
{
    public Guid? TenantId { get; set; }
}
