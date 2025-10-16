# ICMarkets Web API - .NET 8 (SQLite)

This is a scaffolded solution implementing the ICMarkets Web API task:
- .NET 8 Web API
- Uses SQLite (EF Core) to persist raw JSON responses from BlockCypher + CreatedAt timestamp
- Vertical-slices / CQRS-style handlers (no repository pattern; direct DbContext usage per request)
- Swagger, HealthChecks, CORS, FluentValidation placeholder
- Dockerfile included

Build & run (requires .NET 8 SDK):
```bash
dotnet restore
dotnet build
cd src/ICMarkets.Api
dotnet run
```

Swagger: `http://localhost:5000/swagger` (when running)

The project is a starting point: handlers and services fetch from BlockCypher endpoints and store snapshots.
