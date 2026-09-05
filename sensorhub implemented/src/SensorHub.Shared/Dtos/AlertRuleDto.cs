namespace SensorHub.Shared.Dtos;

public record AlertRuleDto(
    Guid Id,
    Guid SensorId,
    string Operator,
    double Threshold,
    bool IsEnabled
);
