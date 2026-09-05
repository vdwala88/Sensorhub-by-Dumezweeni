using SensorHub.Application.Interfaces;
using SensorHub.Infrastructure.Mqtt;

namespace SensorHub.Worker.Workers;

public class MqttListenerWorker : BackgroundService
{
    private readonly MqttClientService _mqtt;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MqttListenerWorker> _logger;

    public MqttListenerWorker(MqttClientService mqtt, IServiceScopeFactory scopeFactory, ILogger<MqttListenerWorker> logger)
    {
        _mqtt = mqtt;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _mqtt.MessageReceived += OnMessageReceivedAsync;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _mqtt.ConnectAsync(stoppingToken);
        await _mqtt.SubscribeAsync("sensorhub/+/+/readings", stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task OnMessageReceivedAsync(string topic, string payload)
    {
        var parts = topic.Split('/');
        if (parts.Length != 4 || !Guid.TryParse(parts[2], out var sensorId))
        {
            _logger.LogWarning("Ignoring message on unrecognized topic {Topic}", topic);
            return;
        }

        if (!double.TryParse(payload, out var value))
        {
            _logger.LogWarning("Ignoring non-numeric payload {Payload} on {Topic}", payload, topic);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var tenantAccessor = scope.ServiceProvider.GetRequiredService<ICurrentTenantAccessor>();
        if (Guid.TryParse(parts[1], out var tenantId))
            tenantAccessor.TenantId = tenantId;

        var sensorService = scope.ServiceProvider.GetRequiredService<ISensorService>();
        await sensorService.RecordReadingAsync(sensorId, value, DateTime.UtcNow);
    }
}
