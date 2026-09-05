using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;

namespace SensorHub.Infrastructure.Mqtt;

public class MqttClientService : IAsyncDisposable
{
    private readonly IMqttClient _client;
    private readonly ILogger<MqttClientService> _logger;
    private readonly string _host;
    private readonly int _port;

    public event Func<string, string, Task>? MessageReceived;

    public MqttClientService(ILogger<MqttClientService> logger, string host, int port)
    {
        _logger = logger;
        _host = host;
        _port = port;
        _client = new MqttFactory().CreateMqttClient();

        _client.ApplicationMessageReceivedAsync += async e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = e.ApplicationMessage.ConvertPayloadToString();
            if (MessageReceived is not null)
                await MessageReceived.Invoke(topic, payload);
        };
    }

    public async Task ConnectAsync(CancellationToken ct = default)
    {
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(_host, _port)
            .WithClientId($"sensorhub-worker-{Guid.NewGuid():N}")
            .WithCleanSession()
            .Build();

        _logger.LogInformation("Connecting to MQTT broker at {Host}:{Port}", _host, _port);
        await _client.ConnectAsync(options, ct);
    }

    public async Task SubscribeAsync(string topicFilter, CancellationToken ct = default)
    {
        await _client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topicFilter).Build(), ct);
        _logger.LogInformation("Subscribed to {Topic}", topicFilter);
    }

    public async Task PublishAsync(string topic, string payload, CancellationToken ct = default)
    {
        var message = new MqttApplicationMessageBuilder().WithTopic(topic).WithPayload(payload).Build();
        await _client.PublishAsync(message, ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_client.IsConnected)
            await _client.DisconnectAsync();
        _client.Dispose();
    }
}
