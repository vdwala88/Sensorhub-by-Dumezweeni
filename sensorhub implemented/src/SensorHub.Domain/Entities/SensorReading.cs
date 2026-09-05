namespace SensorHub.Domain.Entities;

/// <summary>
/// A single data point reported by a sensor at a point in time.
/// </summary>
public class SensorReading
{
    public Guid Id { get; set; }
    public Guid SensorId { get; set; }
    public double Value { get; set; }
    public DateTime Timestamp { get; set; }

    public Sensor? Sensor { get; set; }
}
