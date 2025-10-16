# ICMarkets Web API - .NET 9 (SQLite)

This is a scaffolded solution implementing the ICMarkets Web API task:
- .NET 9 Web API
- Uses SQLite (EF Core) to persist raw JSON responses from BlockCypher + CreatedAt timestamp
- Vertical-slices / CQRS-style handlers
- Repository, UOW patterns
- Swagger, HealthChecks, CORS, FluentValidation placeholder
- Dockerfile included

Build & run (requires .NET 8 SDK):
```bash
dotnet restore
dotnet build
cd src/ICMarkets.Api
dotnet run
```

Swagger: `http://localhost:5001/swagger`
Health check: `https://localhost:5001/health`

The project is a starting point: handlers and services fetch from BlockCypher endpoints and store snapshots.
