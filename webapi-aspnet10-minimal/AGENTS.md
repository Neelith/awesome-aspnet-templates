# ASP.NET 10 Minimal Template - Architecture & Conventions

## Architecture

Clean Architecture with CQRS. .NET 10.0. 3-layer structure (Core combines Domain + Application).

### Dependency Flow

```
WebApi
    └── Infrastructure
            └── Core
```

| Layer | Project | Responsibility |
|---|---|---|
| **Core** | `YourProjectName.Core` | Entities, value objects, CQRS handlers/validators, repository interfaces, decorators, abstractions (`CacheFactoryException`, `IUnitOfWork`), domain event primitives. No external infrastructure deps. |
| **Infrastructure** | `YourProjectName.Infrastructure` | EF Core `ApplicationDbContext`, repository implementations, HybridCache (Redis-backed), `DateTimeProvider`, `CurrentUserService`. References Core. |
| **WebApi** | `YourProjectName.WebApi` | Entry point. Carter endpoints (`ICarterModule`), middleware (`GlobalExceptionHandler`), settings (JWT, Redis), OpenAPI, auth/authz, OTel telemetry, DI composition root. References Core + Infrastructure. |

### CQRS Pattern

Commands/queries in `Features/<Feature>/<Operation>/`:
- `<Operation>Command` / `<Operation>Query` — request DTOs
- `<Operation>CommandHandler` / `<Operation>QueryHandler` — handlers
- `<Operation>CommandValidator` / `<Operation>QueryValidator` — FluentValidation validators
- `<Operation>Response` — response DTOs

Handlers implement `ICommandHandler<T>`, `ICommandHandler<TCommand, TResponse>`, or `IQueryHandler<TQuery, TResponse>` from Neelith.Hermes.

Queries for GET operations. Commands for all other HTTP methods.

### Decorator Pipeline

Handlers wrapped in order:
1. `ValidationDecorator` — runs FluentValidation, returns `Result.Ko` on failure with `400` metadata
2. `TracingDecorator` — starts an OTel `Activity` span per handler, sets `Ok`/`Error` status based on result

Registered via Scrutor `AddHandlerDecorator` in Core layer.

### Repository Pattern

Repository interfaces in `Core/Repositories/<Aggregate>/`. Implementations in `Infrastructure/Persistence/Repositories/`. Repository methods accept repository-specific command/query DTOs in `Commands/` and `Queries/` subfolders with RepositoryCommand or RepositoryQuery suffixes. ALWAYS use the existing convention, DONT add parameters to the method signature, keep them inside the DTOs.

### Domain Model

The project is using Domain Driven Design, so keep rich domain models (entities) and follow the best DDD practices.

### Database

EF Core with Npgsql (PostgreSQL). Soft delete via `AuditableEntity.Deleted` + global query filter (`MarkAsDeleted()`). Auditing: `CreatedAtUtc`, `CreatedBy`, `UpdatedAtUtc`, `UpdatedBy`. Migrations auto-applied on startup via `AddDatabaseMigrationsExtension`; startup fails fast if the database is unreachable. Domain events raised via `Entity.RaiseDomainEvent` are dispatched to `IDomainEventHandler<T>` implementations after `SaveChangesAsync`.

### Caching

HybridCache (`HybridCache`) with Redis L2 backend. L1 in-memory cache built-in. Falls back to L1-only if Redis not configured. `RedisSettings` section configures Redis connection. `CacheFactoryException` propagates DB errors from `GetOrCreateAsync` factory. Cache keys include all query parameters; entries are tagged (`CacheTags`) and write handlers invalidate via `RemoveByTagAsync`.

### Endpoints

Carter modules implementing `ICarterModule`. Auto-discovered via `AddCarter()` / `MapCarter()`. Lowercase URLs enforced. Health checks: `/health/live` anonymous; `/health` and `/health/ready` require authorization because responses include infrastructure details.

### Error Handling

`GlobalExceptionHandler` routes through `IProblemDetailsService` → ProblemDetails (500). W3C trace context (`traceparent`) propagated from inbound requests. `traceId` injected into ProblemDetails via `Activity.Current?.TraceId`. No custom `x-trace` header.

## Naming Conventions

### Projects & Namespaces

- `YourProjectName.Core` — Core layer (Domain + Application)
- `YourProjectName.Infrastructure` — Infrastructure layer
- `YourProjectName.WebApi` — API host
- `YourProjectName.Unit.Tests` — Unit tests
- `YourProjectName.Integration.Tests` — Integration tests
- `YourProjectName.E2E.Tests` — End-to-end tests

### Files & Classes

- **Commands**: `<Action><Entity>Command` (e.g., `CreateWeatherForecastCommand`)
- **Queries**: `<Action><Entity>Query` (e.g., `GetWeatherForecastsQuery`)
- **Handlers**: `<Action><Entity>CommandHandler` / `<Action><Entity>QueryHandler`
- **Validators**: `<Action><Entity>CommandValidator` / `<Action><Entity>QueryValidator`
- **Responses**: `<Action><Entity>Response` or `IdResponse<T>`
- **Repositories**: `I<Entity>Repository` (interface), `<Entity>Repository` (impl)
- **Repository DTOs**: `<Action><Entity>RepositoryCommand` / `<Action><Entity>RepositoryQuery`
- **Settings**: `<Feature>Settings` (e.g., `JwtSettings`, `RedisSettings`)
- **DI Extensions**: `Add<Feature>Extension` (e.g., `AddSettingsExtension`, `AddRepositoriesExtension`)
- **Interfaces**: `I` prefix (e.g., `IWeatherForecastRepository`, `IUnitOfWork`)
- **Constants**: `ErrorConsts`, `CacheTags`, `HealthCheckTags`, `Tags`

### DI Extension Methods

- `Add<Feature>()` — registers feature services, returns `IServiceCollection`. When the name collides with a framework extension, use `Add<Feature>Services()` (e.g., `AddProblemDetailsServices`, `AddHealthCheckServices`)
- `Add<Layer>Services()` — layer-level registration entry point
- `Use<Feature>()` — middleware/configure phase (e.g., `UseOpenApi()`, `UseLogging()`)
- `Map<Feature>Endpoints()` — endpoint mapping phase (e.g., `MapEndpoints()`, `MapHealthCheckEndpoints()`)

### Directory Structure

```
src/
  YourProjectName.Core/
    Constants/           — ErrorConsts (HTTP status code mappings), CacheTags, HealthCheckTags
    ValueObjects/        — Value objects with factory methods + validation
    Extensions/          — ResultExtensions (BadRequest, NotFound, etc.)
    Abstractions/
      Caching/           — CacheFactoryException
      Decorators/        — ValidationDecorator, TracingDecorator (Scrutor)
      Diagnostics/       — ApplicationDiagnostics (ActivitySource)
      Persistence/       — IUnitOfWork interface
    Entities/
      <Aggregate>/       — Entity classes, Errors classes
    Shared/              — Entity, AuditableEntity, IDomainEvent, IDomainEventHandler
    Services/
      Time/              — IDateTimeProvider interface
      User/              — ICurrentUserService interface
    Repositories/
      <Aggregate>/       — I<Entity>Repository interface
        Commands/        — Repository command DTOs
        Queries/         — Repository query DTOs
    Features/
      <Feature>/
        <Operation>/     — Command/Query, Handler, Validator, Response
    DependencyInjection.cs

  YourProjectName.Infrastructure/
    Persistence/
      ApplicationDbContext
      EntityConfigurations/ — IEntityTypeConfiguration + AuditableEntityExtensions
      Repositories/         — Repository implementations + AddRepositoriesExtension
      Migrations/           — EF Core migrations
      AddDatabaseMigrationsExtension.cs
    Caching/             — RedisSettings
    Time/                — DateTimeProvider
    User/                — CurrentUserService
    DependencyInjection.cs

  YourProjectName.WebApi/
    Constants/           — Tags
    Extensions/          — ResultExtensions (Result → ProblemHttpResult)
    Middlewares/         — GlobalExceptionHandler
    Endpoints/           — Carter endpoint modules
    DependencyInjectionExtensions/
      AddSettingsExtension         — Generic settings binding
      AddLoggingExtension          — Serilog config
      AddAuthenticationExtension   — JWT Bearer config
      AddAuthorizationExtension    — Authorization setup
      AddEndpointsExtension        — Carter registration
      AddProblemDetailsExtension   — ProblemDetails customization
      AddOpenApiExtension          — OpenAPI + SwaggerUI
      AddTelemetryExtension        — OTel tracing, metrics, OTLP export
      AddHealthCheckExtension      — Health check registration + mapping
      HealthCheckResponseWriter    — JSON health report writer
    Settings/            — JwtSettings
    DependencyInjection.cs
    Program.cs

tests/
  YourProjectName.Unit.Tests/
  YourProjectName.Integration.Tests/
  YourProjectName.E2E.Tests/
```

## General Rules

- Nullable reference types enabled
- Implicit usings enabled via `GlobalUsings.cs` per project
- `internal` for infrastructure implementations, `public` for abstractions and interfaces
- Constructor injection with primary constructors
- Async methods suffixed with `Async` except handler `Handle`
- EF Core entity configurations in `EntityConfigurations/`
- Lowercase URLs enforced via routing options
- Response types implement `IResponse` from Neelith.Hermes
- Empty validators (no rules) not permitted for non-parameterless handlers
- Domain entity factory methods return `Result<T>`, not exceptions for validation
