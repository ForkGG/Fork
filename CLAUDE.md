# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Fork** is a cross-platform Minecraft Server Manager GUI — a three-tier .NET 10.0 application consisting of:
- **Backend** — ASP.NET Core 10.0 REST API + WebSocket server (port 35565)
- **Common** — Shared .NET 10.0 class library for models/enums/payloads
- **Frontend** — Blazor WebAssembly 10.0 UI

> This is the successor to Fork-legacy (complete rewrite). **Not production-ready** — use Fork-legacy for production.

## Commands

### Setup
```bash
dotnet workload restore   # Restore Blazor WASM workloads (required first time)
dotnet restore            # Restore NuGet packages
```

### Build & Run
```bash
dotnet build                         # Build entire solution
dotnet run --project Backend         # Start backend on http://localhost:35565
```

Swagger UI is available at `http://localhost:35565/swagger` when running.

### Database Migrations (run from solution root)
```bash
dotnet ef migrations add <Name> --project Backend --startup-project Backend
dotnet ef database update --project Backend
```

The SQLite database is stored at `%APPDATA%\ForkApp\persistence\app.db` on Windows.

### Tests
```bash
dotnet test --no-build --verbosity normal
```
There are currently no test projects in the repo.

### Code Quality
Qodana (JetBrains) is used for static analysis via CI. Config in `qodana.yaml`.

## Architecture

### Communication Flow
```
Frontend (Blazor WASM)
  ↕ REST API  →  /v1/application, /v1/entity, /v1/createEntity
  ↕ WebSocket →  real-time notifications (ConsoleAdd, EntityStatusChanged, Player events)
Backend (ASP.NET Core) on localhost:35565
  ↕ SQLite via EF Core (ServerSet, PlayerSet, AppSettingsSet)
```

### Backend (`Backend/src/`)
- **Controllers/** — `ApplicationController`, `EntityController`, `CreateEntityController` (all inherit `AbstractRestController`)
- **Logic/Managers/** — Singleton application-level managers: `ApplicationManager`, `EntityManager`, `ServerVersionManager`, `TokenManager`
- **Logic/Services/** — Domain services grouped by area: Entity, File, State, Authentication, Web
- **Logic/Services/EntityServices/** — Core Minecraft server management: `CommandService`, `ConsoleService`, `EntityService`, `ServerService`, `PlayerService`, etc.
- **Adapters/** — Transient services for external APIs: `MojangApiAdapter`, `PaperMcApiAdapter`, `WaterfallApiAdapter`, `PurpurApiAdapter`, `ForkApiAdapter`
- **Persistence/** — `ApplicationDbContext` (EF Core), migrations in `Backend/Migrations/`
- **Util/** — `AuthenticationMiddleware`, `ForkExceptionFilterAttribute`, `JavaUtils`, `MemoryUtil`, `SwaggerUtils`
- **NotificationCenter** — Singleton managing WebSocket connections (using Fleck library)

### Common (`Common/src/Model/`)
Shared models organized by: enums, payloads (request/response DTOs), privileges, notifications.

### Frontend (`Frontend/`)
- **Razor/** — Blazor components by feature: Entity tabs (Console, Settings, Players), Sidebar, Forms, CreateEntity screen
- **Logic/Services/** — Singleton services: `ApplicationConnectionService`, `EntityConnectionService`, `BackendClient` (HTTP to port 35565), `NotificationService` (WebSocket), `ApplicationStateManager`, `TranslationService`, `ToastManager`
- **wwwroot/** — Static assets; translations at `wwwroot/resources/translation.json`

### Authentication
Token-based. `AuthenticationMiddleware` validates tokens. `TokenManager` issues tokens. REST endpoints require privilege tokens; privilege tree defined in Common models.

### .NET SDK Version
Pinned to `10.0.101` via `global.json`.
