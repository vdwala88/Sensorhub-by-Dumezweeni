namespace SensorHub.Domain.Entities;

/// <summary>
/// A customer/organization using the platform (multi-tenancy root).
/// </summary>
public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty; // e.g. Agriculture, Cement, Retail
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Sensor> Sensors { get; set; } = new List<Sensor>();
}
