namespace SensorHub.Application.Interfaces;

/// <summary>
/// Resolves which tenant the current request/message belongs to.
/// The Api sets this from an "X-Tenant-Id" header; the Worker sets it from
/// the tenant ID embedded in each MQTT topic (sensorhub/{tenantId}/{sensorId}).
/// </summary>
public interface ICurrentTenantAccessor
{
    Guid? TenantId { get; set; }
}
