# SensorHub

A multi-tenant IoT platform for ingesting, storing, and visualizing data from
any type of sensor — temperature, soil moisture, GPS, RFID, PLC, smart meter,
vibration, water level, air quality, and more. Built to be the shared
foundation under multiple industry-specific products (agriculture, cement,
mining, retail, etc.) rather than a single-purpose app.

## What it does

- Ingests sensor readings over MQTT
- Persists readings per tenant (multi-tenant from the ground up)
- Exposes a REST API for querying sensors and readings
- Runs a background worker that evaluates alert rules on incoming data
- Serves a live dashboard (Blazor Server) for visualizing sensor state
- Includes a device simulator so the platform can be developed and demoed
  without physical hardware

## Architecture

Clean Architecture, .NET 8. See [`docs/architecture.md`](docs/architecture.md)
for the full layer breakdown and data flow diagram.

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

Structural scaffold — folders, `.csproj` files, and stub classes with
`TODO`s / `NotImplementedException`. Not yet functional; see
`docs/architecture.md` for the implementation order.

## Requirements

- .NET 8 SDK
- PostgreSQL (or SQL Server)
- An MQTT broker (e.g. Mosquitto) — see `deployment/docker-compose.yml`

## License

See [`LICENSE`](LICENSE).
