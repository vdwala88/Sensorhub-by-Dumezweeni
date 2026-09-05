# SensorHub — Architecture

Clean Architecture, .NET 8. Core platform is sensor-type agnostic (temperature,
soil moisture, GPS, RFID, PLC, smart meter, vibration, water level, air quality).

## Layers

- **Domain** — entities and business rules (`Sensor`, `SensorReading`, `Tenant`, `AlertRule`). No dependencies.
- **Application** — use cases (`SensorService`, `AlertEngine`) behind interfaces (`ISensorService`, `ISensorRepository`, `IAlertEngine`). Depends on Domain + Shared.
- **Infrastructure** — EF Core (`SensorHubDbContext`, SQLite by default / any relational provider in production), MQTT client (MQTTnet). Depends on Application.
- **Api** — REST endpoints (`SensorsController`), tenant-resolution middleware. Depends on Application + Infrastructure.
- **Worker** — background service that listens to MQTT, persists readings, runs alert rules.
- **Web** — Blazor Server dashboard polling the API for live sensor state.
- **Shared** — DTOs shared between Api, Web, and Mobile clients.
- **DeviceSimulator** — publishes fake sensor data to MQTT for local dev/demo without hardware.

## Multi-tenancy

Every `Sensor` row carries a `TenantId`. Rather than relying on each query to
remember a `WHERE TenantId = ...` clause, tenant isolation is enforced once,
centrally, via an EF Core global query filter on `SensorHubDbContext`, driven
by a scoped `ICurrentTenantAccessor`:

- The **Api** sets it from the `X-Tenant-Id` request header (`TenantMiddleware`).
- The **Worker** sets it by parsing the tenant segment out of the MQTT topic.

This means a bug in a controller or a new use case can't accidentally leak
one tenant's sensor data into another tenant's response — the filter applies
at the DbContext level, not per-query.

## Data flow

```
Sensor / DeviceSimulator
        │  MQTT (sensorhub/{tenantId}/{sensorId}/readings)
        ▼
   MQTT Broker (Mosquitto)
        │
        ▼
  Worker (MqttListenerWorker)
        │  RecordReadingAsync → AlertEngine.EvaluateAsync
        ▼
     Database  ◄────────────  REST API (SensorsController)
                                     │
                                     ▼
                          Web Dashboard (polls every 5s)
```

## Status

Core vertical slice is implemented end-to-end: register a sensor, ingest
readings over MQTT or via the REST API, evaluate alert rules, and view live
state on the dashboard. Backed by EF Core InMemory out of the box (zero
config) or SQLite via a connection string.

Not yet implemented (roadmap):
- Persisted alert history / notification delivery (email, SMS, webhook) once a rule triggers
- SignalR push instead of dashboard polling
- Mobile client consuming `SensorHub.Shared` DTOs
- Postgres/SQL Server migrations for production deployments (currently EnsureCreated, fine for dev/demo)
- Auth (API keys or OAuth) — currently tenant is trusted from a header, which is fine for local dev but not production
