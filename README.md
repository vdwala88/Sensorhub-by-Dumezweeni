# SensorHub

![build](https://github.com/vdwala88/Sensorhub-by-Dumezweeni/actions/workflows/build.yml/badge.svg)

A multi-tenant IoT platform for ingesting, storing, and visualizing data from
any type of sensor — temperature, soil moisture, GPS, RFID, PLC, smart meter,
vibration, water level, air quality, and more. Built to be the shared
foundation under multiple industry-specific products (agriculture, cement,
mining, retail, etc.) rather than a single-purpose app.

## What it does

- Ingests sensor readings over MQTT, or via a REST endpoint
- Persists readings per tenant (multi-tenant from the ground up, enforced via
  an EF Core global query filter — see [`docs/architecture.md`](docs/architecture.md))
- Exposes a REST API for registering sensors and querying readings
- Runs a background worker that evaluates alert rules on every incoming reading
- Serves a live dashboard (Blazor Server) for visualizing sensor state
- Includes a device simulator so the platform can be developed and demoed
  without physical hardware

## Architecture

Clean Architecture, .NET 8. See [`docs/architecture.md`](docs/architecture.md)
for the full layer breakdown, multi-tenancy design, and data flow diagram.

```
SensorHub/
├── SensorHub.sln
├── src/
│   ├── SensorHub.Api/              REST API (ASP.NET Core)
│   ├── SensorHub.Application/      Services, use cases, interfaces
│   ├── SensorHub.Domain/           Entities, business rules
│   ├── SensorHub.Infrastructure/   EF Core DbContext, MQTT client
│   ├── SensorHub.Worker/           Background MQTT listener + alert engine
│   ├── SensorHub.Web/              Blazor Server dashboard
│   ├── SensorHub.Shared/           DTOs shared across Api/Web/Mobile
│   └── SensorHub.DeviceSimulator/  Fake sensor data generator for dev/demo
├── tests/
│   └── SensorHub.Tests/
├── docs/
│   └── architecture.md
├── deployment/
│   └── docker-compose.yml
└── README.md
```

## Status

Core vertical slice works end-to-end: register a sensor → ingest a reading
(via MQTT or REST) → evaluate alert rules → view it on the dashboard. Runs
out of the box against an in-memory database with no external dependencies;
point it at SQLite/Postgres and a real MQTT broker for anything persistent.
See [`docs/architecture.md`](docs/architecture.md) for what's implemented vs.
still on the roadmap (auth, notification delivery, SignalR push, mobile client).

## Running locally

**Zero-config (API + Web only, no MQTT):**
```bash
dotnet run --project src/SensorHub.Api      # http://localhost:5000, Swagger at /swagger
dotnet run --project src/SensorHub.Web      # http://localhost:5001
```
The API defaults to an in-memory database, so sensors registered via
Swagger/curl show up on the dashboard immediately.

**Full stack with MQTT ingestion:**
```bash
docker compose -f deployment/docker-compose.yml up -d db mqtt   # Postgres + Mosquitto
dotnet run --project src/SensorHub.Worker
dotnet run --project src/SensorHub.DeviceSimulator -- <tenantId> <sensorId> localhost
```

**Tests:**
```bash
dotnet test
```

## Requirements

- .NET 8 SDK
- PostgreSQL or SQLite (in-memory by default — nothing to install for local dev)
- An MQTT broker (e.g. Mosquitto) — only needed for the Worker/Simulator path; see `deployment/docker-compose.yml`

## License

See [`LICENSE`](LICENSE).
