namespace SensorHub.Domain.Entities;

/// <summary>
/// Represents a single registered sensor/device, regardless of type
/// (temperature, soil moisture, GPS, RFID, PLC, smart meter, etc.).
/// </summary>
public class Sensor
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SensorReading> Readings { get; set; } = new List<SensorReading>();
    public ICollection<AlertRule> AlertRules { get; set; } = new List<AlertRule>();
}
