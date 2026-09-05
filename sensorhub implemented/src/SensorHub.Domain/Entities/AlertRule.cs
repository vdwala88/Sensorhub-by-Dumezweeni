using SensorHub.Domain.Enums;

namespace SensorHub.Domain.Entities;

/// <summary>
/// A threshold rule evaluated against incoming readings for a sensor
/// (e.g. "soil moisture &lt; 30" or "temperature &gt; 80").
/// </summary>
public class AlertRule
{
    public Guid Id { get; set; }
    public Guid SensorId { get; set; }
    public ComparisonOperator Operator { get; set; }
    public double Threshold { get; set; }
    public bool IsEnabled { get; set; } = true;

    public Sensor? Sensor { get; set; }
}
