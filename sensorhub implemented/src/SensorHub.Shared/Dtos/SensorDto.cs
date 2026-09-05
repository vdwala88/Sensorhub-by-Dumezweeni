namespace SensorHub.Shared.Dtos;

public record SensorDto(
    Guid Id,
    string Name,
    string Location,
    string Type,
    bool IsActive
);

public record SensorReadingDto(
    Guid SensorId,
    double Value,
    DateTime Timestamp
);
