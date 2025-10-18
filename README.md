# ICMarkets Web API - .NET 9 (SQLite)

This is a scaffolded solution implementing the ICMarkets Web API task:
- .NET 9 Web API
- Uses SQLite (EF Core) to persist raw JSON responses from BlockCypher + CreatedAt timestamp
- Dependency Injection (Req 5), Asynchronous event patterns (Tasks/Async/Await),
- Clean Architecture, Vertical-slices, CQRS-style handlers
- Repository, UOW, Mediator, Command, Factory patterns
- Swagger, HealthChecks, CORS, FluentValidation
- Dockerfile included
- Unit tests project
- Functional, Integration tests projects (unfinished due to dotnet 9 - packages incompatibility & no capacity for that within context)

Run project api: Build & run main project .Api (dotnet 9) / dotnet run OR dotnet run --project ICMarkets.Api.csproj (inside the repo)
Run tests: run project .Tests.Unity OR dotnet test (inside the repo - Unit tests are supported only for now)
Swagger: `http://localhost:5001/swagger`
Health check: `https://localhost:5001/health`

The project is a starting point: handlers and services fetch from BlockCypher endpoints and store snapshots.

Repo structure:
- main (stable state only)
- develop (development updates)
- note, main & develop branches history might not align with best practices due to context of test

# Task Description & Requirements:

The purpose of this project is to validate the candidate's approach of designing a Web API. The app
stores all data into the database from ETH, Dash, BTC, LTC API(s). Source Blockcypher:
1. https://api.blockcypher.com/v1/eth/main
2. https://api.blockcypher.com/v1/dash/main
3. https://api.blockcypher.com/v1/btc/main
4. https://api.blockcypher.com/v1/btc/test3
5. https://api.blockcypher.com/v1/ltc/main

Documentation: Blockchain API – Blockchain Developer API for Bitcoin, Ethereum, Testnet,
Litecoin and More | BlockCypher

Minimum Functionality needed:
1. .NET Core Application based on clean or vertical architecture with SOLID.
2. API endpoints that show all https requests into Swagger endpoints.The API endpoints must show
the history of each blockchain’s data stored in the database.
3. Storing blockchain’s data into the database with additional timestamp - CreatedAt. A new column
adding the time requested from the API endpoint. The history of data should be shown with the
CreatedAt in descending order.
4. HealthChecks route and basic CORS policy.
5. Dependency injection, logging, model mapping, API serialization, automatic behavior validation.
6. Integration, Functional and Unit Test projects.
7. Runtime profiles on .NET, Docker (Linux) (..)

Frameworks Requirements:
1. Implementation with .NET Core preferrable >= .NET 6
2. Database options of your choice: SQLite (EF) or Database in Docker, or NoSQL.
3. Main data should be stored as provided in theAPI’s JSON responses.
4. The app must illustrate best practices based on performance, inheritance and scalability.
5. Asynchronous or parallel event patterns.e.g Tasks, PLinq (..)
6. Provide at least two design patterns: UOW, Repository, CQRS, Event Sourcing,
7. Optional: API Gateway
Please, the solution should be submitted back to the email by providing a public github URL, a
ReadMe.md file with instructions. The repository should have main and development branches.
