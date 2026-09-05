using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SensorHub.Application.Interfaces;
using SensorHub.Application.Services;
using SensorHub.Infrastructure.Mqtt;
using SensorHub.Infrastructure.Persistence;
using SensorHub.Worker.Workers;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<SensorHubDbContext>(options =>
{
    if (string.IsNullOrWhiteSpace(connectionString))
        options.UseInMemoryDatabase("sensorhub-dev");
    else
        options.UseSqlite(connectionString);
});

builder.Services.AddScoped<ICurrentTenantAccessor, CurrentTenantAccessor>();
builder.Services.AddScoped<ISensorRepository, SensorRepository>();
builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddScoped<IAlertEngine, AlertEngine>();

builder.Services.AddSingleton(sp => new MqttClientService(
    sp.GetRequiredService<ILogger<MqttClientService>>(),
    host: builder.Configuration["Mqtt:Host"] ?? "localhost",
    port: int.TryParse(builder.Configuration["Mqtt:Port"], out var p) ? p : 1883));

builder.Services.AddHostedService<MqttListenerWorker>();

var host = builder.Build();
host.Run();
