# ASP.NET 10 Minimal Web API Template

Clean Architecture + CQRS template: Carter endpoints, EF Core (PostgreSQL) with soft delete and auditing, HybridCache (Redis L2), JWT Bearer auth, OpenTelemetry, Serilog, health checks.

## Usage

```bash
dotnet new install .
dotnet new aspnet-minimal -n MyProject
```

## Run

```bash
docker compose up -d        # postgres, redis, keycloak, aspire dashboard, app
dotnet run --project src/YourProjectName.WebApi
```

Local profiles: `Local` (no HTTPS metadata requirement, self-contained) and `Development`.
Migrations are applied automatically at startup; the app fails to start if the database is unreachable.

## Endpoints

- `GET/POST /weatherforecasts`, `PUT/DELETE /weatherforecasts/{id}` — example CRUD (JWT required)
- `GET /health/live` — anonymous liveness probe
- `GET /health/ready`, `GET /health` — require authentication (they expose infrastructure details)
- `/openapi` — Swagger UI (Local/Development only)

## Configuration

| Setting | Required | Notes |
|---|---|---|
| `ConnectionStrings:YourProjectNameDb` | Yes | PostgreSQL connection string |
| `JwtSettings:Authority` | Yes | JWT authority URL |
| `JwtSettings:Issuer` | Yes | JWT issuer |
| `JwtSettings:Audience` | Yes | JWT audience |
| `RedisSettings:ConnectionString` | No | Redis connection string; falls back to L1-only cache if absent |
| `RedisSettings:KeyPrefix` | No | Redis key prefix |
| `OpenTelemetrySettings:OtlpEndpoint` | No | OTLP collector endpoint |
| `OpenTelemetrySettings:ServiceName` | No | Service name in traces (default: `YourProjectName.WebApi`) |
| `OpenTelemetrySettings:ServiceVersion` | No | Service version in traces (default: `1.0.0`) |
| `Serilog` | No | Serilog configuration section |

Sensitive values should be stored via User Secrets or environment variables. See `src/YourProjectName.WebApi/appsettings.Development.json` for examples.

## ⚠️ Security disclaimer — read before production use

This template ships **development-only** defaults. Before deploying anywhere real:

- **Default credentials** in `docker-compose.yml` / `appsettings.*.json`: postgres `postgres/postgres`, Keycloak `admin/admin`. Replace them.
- **JWT authority uses plain HTTP** against the local Keycloak. `RequireHttpsMetadata` is disabled only for the `Local` environment; keep it enabled everywhere else and use HTTPS issuers.
- **Example endpoints** (`weatherforecasts`) are placeholders to demonstrate the patterns (CQRS, decorators, caching, repository). Delete or rework them for your domain.
- **No CORS policy, rate limiting, or `AllowedHosts` restriction** is configured — set values appropriate for your deployment.
- **Migrations run at application startup** — convenient for dev; for multi-instance production deployments prefer a dedicated migration job/step.
- The Aspire dashboard and OTLP endpoints in `docker-compose.yml` are exposed without authentication — dev only.
