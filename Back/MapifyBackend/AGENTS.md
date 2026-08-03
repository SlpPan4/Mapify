# Mapify Backend — Agent Guide

This document is a quick reference for AI coding agents working on the MapifyBackend project.

## Project Overview

MapifyBackend is an ASP.NET Core Web API for the Mapify application. It stores and serves video-game Rainbox Six Siege strategies data: **maps**, **strategies** ("strats"), **categories**, and **operators**. It also supports a public submission workflow where users can propose new strats/categories; proposals land in pending tables and are promoted to public tables only after admin approval.

The solution contains the main web project (`MapifyBackend/MapifyBackend.csproj`) and an xUnit integration-test project (`MapifyBackend.IntegrationTests/MapifyBackend.IntegrationTests.csproj`).

## Technology Stack

- **Runtime / Framework:** .NET 10 (`net10.0`), ASP.NET Core Web API
- **Language:** C# 13 with `ImplicitUsings` and `Nullable` enabled
- **Database:** SQLite via `Microsoft.Data.Sqlite`
- **Data access:** Dapper (`Dapper` 2.1.72)
- **ORM packages present but not actively used in code:** Entity Framework Core 10.0.5 (SQLite and Npgsql providers are referenced; no `DbContext` is wired up in `Program.cs`)
- **Serialization:** System.Text.Json with `JsonStringEnumConverter` for enum output
- **IDE metadata:** JetBrains Rider / IntelliJ IDEA files under `.idea/`

## Project Structure

```text
MapifyBackend/
├── Program.cs                              # Entry point, DI registration, middleware
├── MapifyBackend.csproj                    # Project file and NuGet references
├── Controllers/                            # ASP.NET Core API controllers
│   ├── CategoriesController.cs
│   ├── OperatorsController.cs
│   ├── StratsController.cs
│   └── SubmissionsController.cs
├── database_files/
│   ├── DatabaseInitializer.cs              # Runs schema script on startup
│   ├── DatabaseService.cs                  # All Dapper/SQLite data access
│   ├── mainschema.sql                      # Schema + seed data
│   ├── Entities/                           # Plain domain models
│   │   ├── Category.cs
│   │   ├── CategorySubmission.cs
│   │   ├── Map.cs
│   │   ├── Operator.cs
│   │   ├── Strat.cs
│   │   └── StratSubmission.cs
│   └── Services/                           # Thin service layer
│       ├── CategoryService.cs
│       ├── OperatorService.cs
│       ├── StratService.cs
│       └── SubmissionService.cs
└── Utility/
    ├── InputValidator.cs                   # Request validation helpers
    ├── DataNormalizingHelpers/StringHelper.cs
    ├── DTOs/                               # API request models
    │   ├── CategoryRequest.cs
    │   ├── CategorySubmissionRequest.cs
    │   ├── StratRequest.cs
    │   └── StratSubmissionRequest.cs
    └── Enums/Side.cs                       # Attack / Defense enum

MapifyBackend.IntegrationTests/
├── ControllerTestsBase.cs                  # Per-test WebApplicationFactory setup
├── CustomWebApplicationFactory.cs          # Isolated temp-database factory
├── HttpResponseMessageExtensions.cs        # Test JSON helpers (includes enum converter)
├── CategoriesControllerTests.cs
├── OperatorsControllerTests.cs
├── StratsControllerTests.cs
└── SubmissionsControllerTests.cs
```

## Runtime Architecture

1. `Program.cs` builds the web app, registers services, enables CORS (`AllowAll`), and maps controllers.
2. On startup, `DatabaseInitializer.EnsureDatabaseCreated()` runs `mainschema.sql` against `database.db` in the application base directory, creating tables and seeding maps, operators, categories, and a few sample strats.
3. `DatabaseService` is registered as a singleton and opens a new `SqliteConnection` per operation, enabling `PRAGMA FOREIGN_KEYS = ON` each time.
4. Service classes are registered as scoped and contain the application logic between controllers and `DatabaseService`.
5. Controllers accept JSON requests, call validators/services, and return JSON responses.

### Dependency Injection Lifetimes

- `DatabaseService` — Singleton
- `StratService`, `CategoryService`, `OperatorService`, `SubmissionService` — Scoped

## Build, Run, and Test

### Requirements

- .NET 10 SDK (verified: `10.0.302`)

If the SDK is not installed, options include:

- **Installer:** download the .NET 10 SDK from https://dotnet.microsoft.com/download/dotnet/10.0 and run it.
- **winget:** `winget install Microsoft.DotNet.SDK.10`
- **Install script (PowerShell):**
  ```powershell
  Invoke-WebRequest -Uri https://dot.net/v1/dotnet-install.ps1 -OutFile dotnet-install.ps1
  .\dotnet-install.ps1 -Channel 10.0
  ```

After installation, verify with `dotnet --version`.

### Build the solution

```bash
dotnet build MapifyBackend.sln
```

### Run the API locally

```bash
dotnet run --project MapifyBackend/MapifyBackend.csproj
```

By default ASP.NET Core will listen on `http://localhost:{port}` (the exact port is chosen by Kestrel if no `launchSettings.json` is present; the repository does not contain one).

### Test projects

The solution includes `MapifyBackend.IntegrationTests`, an xUnit project that uses `WebApplicationFactory<Program>` with an isolated temp-file SQLite database for each test.

Run the tests:

```bash
dotnet test MapifyBackend.sln
```

New controller tests should follow the existing pattern in `MapifyBackend.IntegrationTests/*ControllerTests.cs`: inherit `ControllerTestsBase`, use `Client` to call the API, and deserialize responses with `ReadApiResponseAsync<T>()` (it configures `JsonStringEnumConverter` for the `Side` enum).

## API Routing Summary

All controllers live under `/api`:

- `/api/strats` — read/create/delete strats, get strats by category/map/operator, assign categories
- `/api/categories` — read/create/delete categories
- `/api/operators` — read operators, assign/remove operators from strats
- `/api/submissions` — public user submissions
  - `POST /api/submissions/strats`
  - `POST /api/submissions/categories`
- `/api/submissions/admin/...` — pending submission review (list, approve, reject)

See `API_DOCUMENTATION.md` for full request/response details.

## Code Style and Conventions

- Use the existing namespace style: `MapifyBackend.<folder>`.
- Entity/DTO property names use PascalCase. Some JSON output uses camelCase aliases mapped via Dapper (`videoUrl`, `mapId`, etc.). Entity properties have public setters so they can be both mapped by Dapper and round-tripped through `System.Text.Json` in integration tests.
- The `Side` enum is serialized as a JSON string (`Attack`, `Defense`).
- Map names are normalized with `StringHelper.Capitalize` in request DTOs (`StratRequest`). Public submissions use `MapId` instead.
- All database I/O in `DatabaseService` and the service layer is asynchronous (`async`/`await`). Match the existing pattern when adding new data-access methods.
- Controllers return `IActionResult` and wrap errors in anonymous objects like `new { error = "..." }` or `new { message = "..." }`.

## Important Implementation Notes

- **No authentication/authorization** is implemented yet. Admin submission endpoints are explicitly marked as unsafe for public exposure in `API_DOCUMENTATION.md` and `SubmissionsController.cs` comments.
- **CORS is wide open:** `AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()`.
- **Database is file-based SQLite** (`database.db`) created in the app output folder. It is re-initialized every startup by running `mainschema.sql` (the script uses `CREATE TABLE IF NOT EXISTS` and `INSERT OR IGNORE`, so existing data is preserved).
- **EF Core packages are referenced but not wired up.** If you add EF migrations or a `DbContext`, you will be introducing a new pattern; do not assume one already exists.
- **`mainschema.sql` is the source of truth** for schema and seed data. If you change the schema, update this file and consider whether existing seed data needs adjustment.

## Security Considerations

- The SQLite package transitively pulls in `SQLitePCLRaw.lib.e_sqlite3` 2.1.11, which currently triggers NuGet advisory `GHSA-2m69-gcr7-jv3q` (high severity). Address this when feasible, likely by updating SQLite-related packages.
- Do not expose the admin submission routes publicly without adding authentication/authorization.
- User input is validated in `InputValidator`, but SQL is written manually with Dapper parameters. Continue using parameterized queries; never concatenate user input into SQL strings.
- CORS policy allows all origins — tighten this before production deployment.
- The SQLite database file (`database.db`) sits in the application directory and is created with default permissions. For production, use a persistent volume and proper file permissions.

## Common Tasks

### Add a new API endpoint

1. Add or extend a controller in `MapifyBackend/Controllers/`.
2. Implement business logic in the matching service under `MapifyBackend/database_files/Services/`.
3. Add raw SQL methods to `MapifyBackend/database_files/DatabaseService.cs`.
4. If the request shape changes, add/update a DTO in `MapifyBackend/Utility/DTOs/` and validation in `InputValidator.cs`.
5. Update `API_DOCUMENTATION.md` if the endpoint is user-facing.

### Change the database schema

1. Edit `MapifyBackend/database_files/mainschema.sql`.
2. If tables are new, Dapper queries in `DatabaseService.cs` will need matching SELECT/INSERT/UPDATE methods.
3. Rebuild and run; `DatabaseInitializer` will apply the script on startup.

### Add tests

Add test classes to the existing `MapifyBackend.IntegrationTests` project:

1. Inherit `ControllerTestsBase` to get a fresh `WebApplicationFactory`/`HttpClient` per test.
2. Call endpoints through `Client` and assert status codes / response envelopes.
3. For typed responses that include the `Side` enum, use `response.ReadApiResponseAsync<T>()` from `HttpResponseMessageExtensions` so the enum string values deserialize correctly.
4. Avoid sharing mutable state between tests; the base class creates a new isolated database for each test.

Only create a new test project if you need unit tests that do not require the full ASP.NET Core host.
