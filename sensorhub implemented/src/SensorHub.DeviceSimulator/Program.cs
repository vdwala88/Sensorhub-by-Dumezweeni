// Generates fake sensor readings and publishes them to the MQTT broker so
// the platform can be developed and demoed without physical hardware.
// Usage: dotnet run --project src/SensorHub.DeviceSimulator -- <tenantId> <sensorId> <mqttHost>

using MQTTnet;
using MQTTnet.Client;

var tenantId = args.Length > 0 ? args[0] : Guid.NewGuid().ToString();
var sensorId = args.Length > 1 ? args[1] : Guid.NewGuid().ToString();
var host = args.Length > 2 ? args[2] : "localhost";
const int port = 1883;

var topic = $"sensorhub/{tenantId}/{sensorId}/readings";
Console.WriteLine($"SensorHub.DeviceSimulator — publishing to {host}:{port} on topic {topic}");

var factory = new MqttFactory();
using var client = factory.CreateMqttClient();

var options = new MqttClientOptionsBuilder()
    .WithTcpServer(host, port)
    .WithClientId($"sensorhub-simulator-{Guid.NewGuid():N}")
    .Build();

await client.ConnectAsync(options, CancellationToken.None);
Console.WriteLine("Connected. Publishing a simulated reading every 5 seconds. Ctrl+C to stop.");

var random = new Random();
while (true)
{
    var value = Math.Round(20 + random.NextDouble() * 15, 2);
    var message = new MqttApplicationMessageBuilder()
        .WithTopic(topic)
        .WithPayload(value.ToString())
        .Build();

    await client.PublishAsync(message, CancellationToken.None);
    Console.WriteLine($"Published {value} to {topic}");

    await Task.Delay(TimeSpan.FromSeconds(5));
}
