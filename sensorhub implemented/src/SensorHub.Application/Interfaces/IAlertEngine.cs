namespace SensorHub.Application.Interfaces;

public interface IAlertEngine
{
    Task<IReadOnlyList<Guid>> EvaluateAsync(Guid sensorId, double value, CancellationToken ct = default);
}
