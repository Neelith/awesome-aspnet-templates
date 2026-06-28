# TODO

## 1. Refactor logging to use OTel — remove custom traceid and manual logging

- [x] Add OpenTelemetry packages (`OpenTelemetry.Extensions.Hosting`, `OpenTelemetry.Exporter.Console`/Zipkin/OTLP)
- [x] Configure OTel in `Program.cs`: add `OpenTelemetryBuilder` with ASP.NET Core instrumentation + `ILogger` integration
- [x] Replace `TraceMiddleware` (custom `x-trace` header / `LogContext.PushProperty`) with OTel `ActivitySource` — keep `x-trace` header via `Activity.Current?.Id`
- [x] Remove `LoggingDecorator` (manual `LogInformation`/`LogError` before/after each handler) — replace with OTel activity events + native `ILogger` scopes
- [x] Update `GlobalExceptionHandler`: remove manual `traceId` extraction from header — use `httpContext.TraceIdentifier` or `Activity.Current?.Id`
- [x] Remove `Serilog.Context` dependency from WebApi
- [x] Update `appsettings.*.json` — remove `TraceIdentifier` from Serilog output templates, rely on OTel-enriched logging

## 2. Infrastructure — docker-compose + health checks

### docker-compose.yml
- [ ] Create `docker-compose.yml` with:
  - `postgres:17-alpine` — port 5432, healthcheck, persistent volume
  - `redis:7-alpine` — port 6379, healthcheck, persistent volume
  - `keycloak:26.1` — port 8080, dev mode, uses postgres as KC DB
  - Named volumes for all three services
  - Container names match existing `appsettings.Local.json` references (`postgres-compose`, `redis-compose`, `keycloak-compose`)

### Health checks
- [] Added `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore` package to WebApi
- [] Created `HealthChecks/RedisHealthCheck.cs` — pings Redis via `IDistributedCache`
- [] Created `HealthChecks/DatabaseHealthCheck.cs` — pings PostgreSQL via `SELECT 1` with Npgsql
- [] Created `DependencyInjectionExtensions/AddHealthCheckExtension.cs` — registers both checks, maps `GET /health`, writes JSON response
- [] Wired `AddAppHealthChecks(dbConnectionString)` and `UseAppHealthChecks()` into `DependencyInjection.cs`

## 3. Add dotnet CLI templates to create CQRS handlers

- [ ] Create `scripts/new-feature.sh` — bash script that scaffolds a new feature with command/query, handler, validator, response, and endpoint files
- [ ] Or create `.template.config/template.json` for `dotnet new` CQRS-handler template
- [ ] Handle `YourProjectName` -> actual project name replacement
- [ ] Preserve existing naming conventions (command/query, handler, validator, response suffixes)

## 4. Add skills for agents to work with the project dd-- NOT SURE, TO REWRITE

- [ ] Create `.opencode/skills/aspnet-minimal/SKILL.md` with:
  - Template architecture overview (3 layers, CQRS, dependency flow)
  - Naming conventions reference
  - Commands for scaffolding new features, running tests, adding migrations
  - Reference to `AGENTS.md` for full details 

## 5. Add setup for test projects

### Unit Tests (`YourProjectName.Unit.Tests`)
- [ ] Add `GlobalUsings.cs` with common namespaces (YourProjectName.Core, FluentAssertions, etc.)
- [ ] Add test framework packages: FluentAssertions, AutoFixture, Shouldly (optional)
- [ ] Implement unit tests for Core layer:
  - [ ] `Features/WeatherForecasts/` — handler + validator tests
  - [ ] Entity factory method tests
  - [ ] Value object validation tests
  - [ ] `TracingDecorator` / `ValidationDecorator` tests
- [ ] Add `Usings.cs` per test class convention

### Integration Tests (`YourProjectName.Integration.Tests`)
- [ ] Add `WebApplicationFactory<Program>` or `IntegrationTestWebApplicationFactory`
- [ ] Add test containers or `Testcontainers` package for PostgreSQL
- [ ] Add `CustomWebApplicationFactory` that:
  - Overrides `DbContext` with test container connection string
  - Applies migrations at startup
  - Seeds test data
- [ ] Implement integration tests for:
  - [ ] Repository layer (CRUD operations against real PG)
  - [ ] CQRS handlers end-to-end (handler → repository → DB)
  - [ ] `IUnitOfWork` behavior (commit/rollback)

### E2E Tests (`YourProjectName.E2E.Tests`)
- [ ] Add `WebApplicationFactory<Program>` with test auth fixture
- [ ] Add `TestAuthHandler` or JWT token fixture for authenticated requests
- [ ] Implement E2E tests for:
  - [ ] `GET /weatherforecasts` — returns 200 with list
  - [ ] `POST /weatherforecasts` — returns 201 with created resource
  - [ ] `GET /weatherforecasts/{id}` — returns 200 or 404
  - [ ] Auth-protected endpoints return 401 without token
- [ ] Add health check endpoint test

## 6. Test the whole application

- [ ] Verify `dotnet restore` succeeds for all projects (src + tests)
- [ ] Verify `dotnet build --no-restore` succeeds — no warnings
- [ ] Run unit tests: `dotnet test tests/YourProjectName.Unit.Tests`
- [ ] Run integration tests (requires PostgreSQL): `dotnet test tests/YourProjectName.Integration.Tests`
- [ ] Run E2E tests: `dotnet test tests/YourProjectName.E2E.Tests`
- [ ] Verify `dotnet run --project src/YourProjectName.WebApi` starts without errors
- [ ] Verify OpenAPI/swagger endpoint returns 200
- [ ] Verify `GET /weatherforecasts` returns expected response
- [ ] Verify `POST /weatherforecasts` creates resource
- [ ] Verify `/health` endpoint returns healthy status
- [ ] Verify OTel telemetry export (console / OTLP) on request flow
- [ ] Verify `x-trace` header still present (via OTel Activity, not custom middleware)
- [ ] Final `dotnet build` with TreatWarningsAsErrors — zero warnings
