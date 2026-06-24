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
| **Core** | `YourProjectName.Core` | Entities, value objects, CQRS handlers/validators, repository interfaces, decorators, abstractions (`IRedisCache`, `IUnitOfWork`), domain event primitives. No external infrastructure deps. |
| **Infrastructure** | `YourProjectName.Infrastructure` | EF Core `ApplicationDbContext`, repository implementations, Redis cache, `DateTimeProvider`, `CurrentUserService`. References Core. |
| **WebApi** | `YourProjectName.WebApi` | Entry point. Carter endpoints (`IEndpoints`), middleware (`TraceMiddleware`, `GlobalExceptionHandler`), settings (JWT, Redis), OpenAPI, auth/authz, DI composition root. References Core + Infrastructure. |

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
2. `LoggingDecorator` — logs before/after execution with errors on failure

Registered via Scrutor `AddHandlerDecorator` in Core layer.

### Repository Pattern

Repository interfaces in `Core/Repositories/<Aggregate>/`. Implementations in `Infrastructure/Persistence/Repositories/`. Repository methods accept repository-specific command/query DTOs in `Commands/` and `Queries/` subfolders with RepositoryCommand or RepositoryQuery suffixes. ALWAYS use the existing convention, DONT add parameters to the method signature, keep them inside the DTOs.

### Domain Model

The project is using Domain Driven Design, so keep rich domain models (entities) and follow the best DDD practices.

### Database

EF Core with Npgsql (PostgreSQL). Soft delete via `AuditableEntity.Deleted` + global query filter. Auditing: `CreatedAtUtc`, `CreatedBy`, `UpdatedAtUtc`, `UpdatedBy`. Migrations auto-applied on startup via `AddDatabaseMigrationsExtension`.

### Caching

Redis via `IRedisCache` interface. Falls back to `IDistributedMemoryCache` if `RedisSettings` not configured. Configured through `RedisSettings` section.

### Endpoints

Carter modules implementing `IEndpoints : ICarterModule`. Auto-discovered via `AddCarter()` / `MapCarter()`. Lowercase URLs enforced.

### Error Handling

`GlobalExceptionHandler` catches unhandled exceptions → ProblemDetails (500). `TraceMiddleware` adds `x-trace` header to all responses. `ProblemDetails` configured with traceId, method, endpoint info.

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
- **Constants**: `ErrorConsts`, `Headers`, `Tags`

### DI Extension Methods

- `Add<Feature>()` — registers feature services, returns `IServiceCollection`
- `Add<Layer>Services()` — layer-level registration entry point
- `Use<Feature>()` — middleware/configure phase (e.g., `UseOpenApi()`, `UseLogging()`)

### Directory Structure

```
src/
  YourProjectName.Core/
    Constants/           — ErrorConsts (HTTP status code mappings)
    ValueObjects/        — Value objects with factory methods + validation
    Extensions/          — ResultExtensions (BadRequest, NotFound, etc.)
    Abstractions/
      Caching/           — IRedisCache interface
      Decorators/        — ValidationDecorator, LoggingDecorator (Scrutor)
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
    Caching/             — RedisCache, RedisSettings
    Time/                — DateTimeProvider
    User/                — CurrentUserService
    DependencyInjection.cs

  YourProjectName.WebApi/
    Constants/           — Headers (x-trace), Tags
    Extensions/          — ResultExtensions (Result → ProblemHttpResult)
    Middlewares/         — GlobalExceptionHandler, TraceMiddleware
    Endpoints/           — IEndpoints interface, Carter endpoint modules
    DependencyInjectionExtensions/
      AddSettingsExtension         — Generic settings binding
      AddLoggingExtension          — Serilog config
      AddAuthenticationExtension   — JWT Bearer config
      AddAuthorizationExtension    — Authorization setup
      AddEndpointsExtension        — Carter registration
      AddProblemDetailsExtension   — ProblemDetails customization
      AddOpenApiExtension          — OpenAPI + SwaggerUI
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
