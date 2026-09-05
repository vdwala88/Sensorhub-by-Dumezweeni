using SensorHub.Application.Interfaces;
using SensorHub.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SensorHub.Application.Services;

public class AlertEngine : IAlertEngine
{
    private readonly ISensorRepository _repository;
    private readonly ILogger<AlertEngine> _logger;

    public AlertEngine(ISensorRepository repository, ILogger<AlertEngine> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Guid>> EvaluateAsync(Guid sensorId, double value, CancellationToken ct = default)
    {
        var rules = await _repository.GetActiveRulesForSensorAsync(sensorId, ct);
        var triggered = new List<Guid>();

        foreach (var rule in rules)
        {
            var isTriggered = rule.Operator switch
            {
                ComparisonOperator.GreaterThan => value > rule.Threshold,
                ComparisonOperator.LessThan => value < rule.Threshold,
                ComparisonOperator.Equal => Math.Abs(value - rule.Threshold) < 0.0001,
                _ => false
            };

            if (isTriggered)
            {
                triggered.Add(rule.Id);
                _logger.LogWarning(
                    "Alert rule {RuleId} triggered for sensor {SensorId}: value {Value} {Operator} {Threshold}",
                    rule.Id, sensorId, value, rule.Operator, rule.Threshold);
            }
        }

        return triggered;
    }
}
