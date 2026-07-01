# TODO

### Health checks
- [x] Added `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore` package to Infrastructure
- [x] Created `HealthChecks/RedisHealthCheck.cs` — pings Redis via `IConnectionMultiplexer.GetDatabase().PingAsync()`
- [x] Used `AddDbContextCheck<ApplicationDbContext>` for PostgreSQL probe (no custom DatabaseHealthCheck needed)
- [x] Added `AddHealthChecks(redisEnabled)` to Infrastructure `DependencyInjection.cs` — conditional Redis check
- [x] Created `DependencyInjectionExtensions/AddHealthCheckExtension.cs` — registers `self` liveness check, maps `/health/live`, `/health/ready`, `/health` with JSON response writer
- [x] Wired `AddAppHealthChecks(redisEnabled)` and `UseAppHealthChecks()` into `DependencyInjection.cs`
- [x] Added `Liveness`/`Readiness` constants to `Tags.cs`
- [ ] Add integration/E2E test for `/health` endpoints

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
