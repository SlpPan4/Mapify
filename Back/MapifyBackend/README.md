# MapifyBackend

ASP.NET Core Web API for the Mapify project. Stores and serves Rainbow Six Siege strategy data: maps, strats, categories, operators, and a public submission workflow with admin approval.

## Tech Stack

- **Runtime:** .NET 10 (`net10.0`)
- **Framework:** ASP.NET Core Web API
- **Database:** SQLite via `Microsoft.Data.Sqlite`
- **Data Access:** Dapper 2.1.72
- **Serialization:** System.Text.Json with `JsonStringEnumConverter`
- **Tests:** xUnit integration tests using `WebApplicationFactory<Program>`

## Project Structure

```text
MapifyBackend/
├── Program.cs                              # Entry point, DI, middleware, CORS
├── MapifyBackend.csproj                    # Project file and NuGet references
├── Controllers/                            # API controllers
│   ├── CategoriesController.cs
│   ├── MapsController.cs
│   ├── OperatorsController.cs
│   ├── StratsController.cs
│   └── SubmissionsController.cs
├── database_files/
│   ├── DatabaseInitializer.cs              # Runs schema script on startup
│   ├── DatabaseService.cs                  # All Dapper/SQLite data access
│   ├── mainschema.sql                      # Schema + seed data
│   ├── Entities/                           # Plain domain models
│   └── Services/                           # Thin service layer
└── Utility/
    ├── Api/ApiResponse.cs                  # Generic API response envelope
    ├── InputValidator.cs                   # Request validation
    ├── DataNormalizingHelpers/StringHelper.cs
    ├── DTOs/                               # API request/response models
    └── Enums/Side.cs                       # Attack / Defense enum

MapifyBackend.IntegrationTests/
├── ControllerTestsBase.cs
├── CustomWebApplicationFactory.cs
├── HttpResponseMessageExtensions.cs
└── *ControllerTests.cs
```

## Build & Run

Requires .NET 10 SDK.

```bash
# Build the solution
dotnet build MapifyBackend.sln

# Run the API
dotnet run --project MapifyBackend/MapifyBackend.csproj
```

By default Kestrel listens on `http://localhost:{port}` (no `launchSettings.json` is included, so the port is chosen automatically).

## Run Tests

```bash
dotnet test MapifyBackend.sln
```

Integration tests use a temporary SQLite database per test via a custom `WebApplicationFactory`.

## API Overview

All endpoints live under `/api`. Full request/response details are in [`API_DOCUMENTATION.md`](API_DOCUMENTATION.md).

### Public read endpoints

- `GET /api/strats` — list strats, optional filters: `name`, `mapId`, `categoryId`, `operatorId`
- `GET /api/strats/{id}` — full strat details (map, categories, operators)
- `GET /api/strats/category/{id}`
- `GET /api/strats/bymap/{id}`
- `GET /api/strats/byoperator/{id}`
- `GET /api/strats/maps/{id}`
- `GET /api/strats/maps/byname/{mapName}`
- `GET /api/categories`
- `GET /api/categories/{id}`
- `GET /api/operators`
- `GET /api/operators/{id}`
- `GET /api/operators/by-name/{name}`

### Trusted/admin mutations

- `POST /api/strats` → `201 Created`
- `PUT /api/strats/{id}`
- `PATCH /api/strats/{id}`
- `DELETE /api/strats/{id}`
- `POST /api/strats/assign/strat/{stratId}/category/{categoryId}`
- `POST /api/categories` → `201 Created`
- `DELETE /api/categories/{id}`
- `POST /api/operators/{operatorId}/assign/{stratId}`
- `DELETE /api/operators/{operatorId}/remove/{stratId}`

### Public submissions

- `POST /api/submissions/strats` → `201 Created`
- `POST /api/submissions/categories` → `201 Created`

### Admin review (pending submissions)

- `GET /api/submissions/admin/strats`
- `GET /api/submissions/admin/strats/{id}`
- `POST /api/submissions/admin/strats/{id}/approve`
- `DELETE /api/submissions/admin/strats/{id}`
- `GET /api/submissions/admin/categories`
- `GET /api/submissions/admin/categories/{id}`
- `POST /api/submissions/admin/categories/{id}/approve`
- `DELETE /api/submissions/admin/categories/{id}`

## Response Format

All responses use a single envelope:

```json
{
  "status": 200,
  "data": { ... },
  "message": null,
  "error": null
}
```

- `status` — HTTP status code.
- `data` — payload on success.
- `message` — human-readable summary.
- `error` — error description.

## Important Notes

- **API-key authentication:** admin submission routes (`/api/submissions/admin/...`) and all mutating requests (POST/PUT/PATCH/DELETE) to `/api/strats`, `/api/categories`, `/api/operators` require the `X-Api-Key` header (`ApiKeyAuthMiddleware`). The key comes from `AdminApi:Key` (env var `AdminApi__Key`); in Development it falls back to the dev key `mapify-dev-admin-key`, and outside Development the app fails fast if the key is missing or equals the dev key. Public reads and `POST /api/submissions/...` need no key.
- **Clients:** the admin panel sends the key via `VITE_API_KEY`. The Discord and Telegram bots deliberately do **not** have the key — they only read data and submit new content through the public submissions endpoints.
- **Strategy submissions validate side consistency:** when submitting a strategy, all selected `categoryIds` must share the same `side`, all selected `operatorIds` must share the same `side`, and the two sides must match if both lists are provided.
- **CORS** uses the named `AppCors` policy with origins from `Cors:AllowedOrigins` (env vars `Cors__AllowedOrigins__0`, `__1`, ...). An empty list means allow-all in Development and deny-by-default in Production.
- **Database** is file-based SQLite (`database.db`) initialized from `mainschema.sql` on every startup. Existing data is preserved thanks to `CREATE TABLE IF NOT EXISTS` and `INSERT OR IGNORE`.
- **NuGet advisory:** `SQLitePCLRaw.lib.e_sqlite3` 2.1.11 triggers high-severity advisory [GHSA-2m69-gcr7-jv3q](https://github.com/advisories/GHSA-2m69-gcr7-jv3q). Update SQLite-related packages when feasible.
