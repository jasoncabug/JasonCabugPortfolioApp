# PortfolioApp - Project Context & Architecture

Enterprise-grade **.NET 10 Web API** built with **Clean Architecture**, **CQRS**, and **ASP.NET Core Identity**. This document is the single source of truth for solution architecture, technical decisions, current state, and upcoming work.

---

## Tech Stack & Dependencies

| Area | Choice |
|------|--------|
| **Framework** | .NET 10 (C#) |
| **Architecture** | Clean Architecture (Domain-Driven Design principles) |
| **Pattern** | CQRS via **MediatR** |
| **Database & ORM** | Entity Framework Core 10 + SQL Server (LocalDB for dev) |
| **Authentication** | ASP.NET Core Identity Core + JWT Bearer tokens |
| **Validation** | **FluentValidation** with MediatR pipeline behavior |
| **API Documentation** | Scalar OpenAPI (`Scalar.AspNetCore`) |
| **Error Handling** | `IExceptionHandler` returning RFC 7807 Problem Details |

### Key NuGet Packages

- **MediatR** 14.x — CQRS command/query dispatch
- **FluentValidation** 12.x — request validation
- **Microsoft.EntityFrameworkCore.SqlServer** 10.x — persistence
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore** 10.x — user management
- **Microsoft.AspNetCore.Authentication.JwtBearer** 10.x — token validation
- **Scalar.AspNetCore** 2.x — interactive API docs UI

---

## Solution Structure

```text
JasonCabugPortfolioApp/
├── PortfolioApp/                          # Domain layer (project: PortfolioApp.Domain)
│   ├── Common/
│   │   └── BaseAuditableEntity.cs         # Id, CreatedAt/By, LastModifiedAt/By
│   └── Entities/
│       ├── ApplicationUser.cs             # Custom Identity user (single source of truth)
│       ├── Project.cs                     # Portfolio project entity
│       ├── Skill.cs                       # Skill linked to projects
│       └── WorkExperience.cs              # Work history entity
│
├── PortfolioApp.Application/
│   ├── Auth/                              # LoginRequest, RegisterRequest, AuthResponse
│   ├── Common/
│   │   ├── Behaviors/
│   │   │   └── ValidationBehavior.cs      # MediatR FluentValidation pipeline
│   │   ├── Exceptions/
│   │   │   ├── BadRequestException.cs
│   │   │   └── UnauthorizedException.cs
│   │   └── Interfaces/
│   │       ├── IApplicationDbContext.cs
│   │       ├── IAuthService.cs
│   │       └── ICurrentUserService.cs
│   ├── Projects/
│   │   ├── Commands/CreateProject/        # Command, handler, validator
│   │   └── Queries/                       # GetProjects, GetProjectById, ProjectDto
│   └── DependencyInjection.cs
│
├── PortfolioApp.Infrastructure/
│   ├── Authentication/
│   │   └── AuthService.cs                 # Register, login, JWT generation
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs        # EF Core DbContext + audit fields
│   │   ├── ApplicationDbContextInitialiser.cs  # Migrate + seed on startup
│   │   └── Configurations/                # EF entity configurations
│   ├── Services/
│   │   └── CurrentUserService.cs          # Reads authenticated user from JWT claims
│   ├── Migrations/                        # EF Core migrations
│   └── DependencyInjection.cs
│
└── PortfolioApp.Api/
    ├── Controllers/
    │   ├── ApiControllerBase.cs           # Shared MediatR sender access
    │   ├── AuthController.cs              # Register / Login / Me
    │   └── ProjectsController.cs          # MediatR endpoint entry points
    ├── Infrastructure/
    │   └── GlobalExceptionHandler.cs      # RFC 7807 Problem Details
    ├── Program.cs                         # Startup, Scalar, middleware pipeline
    └── appsettings.json                   # Connection string, JwtSettings
```

### Layer Dependencies

```text
PortfolioApp.Api
    └── PortfolioApp.Infrastructure
            └── PortfolioApp.Application
                    └── PortfolioApp.Domain
```

The Application layer depends only on Domain. Infrastructure implements Application interfaces. The API layer wires everything together via DI extension methods.

---

## Key Architectural Decisions

### 1. Identity Entity Standardization

**Decision:** Use a single `ApplicationUser` in `PortfolioApp.Domain.Entities`.

All Identity registration (`AddIdentityCore`), EF Core (`IdentityDbContext<ApplicationUser>`), `AuthService`, and database seeding reference this one domain type. No duplicate user classes in Infrastructure or API.

### 2. CQRS via MediatR

Controllers stay thin — they send commands/queries through `ISender` and return results. Business logic lives in Application-layer handlers:

- **Commands:** `CreateProjectCommand` → `CreateProjectCommandHandler`
- **Queries:** `GetProjectsQuery`, `GetProjectByIdQuery`

### 3. FluentValidation Pipeline

Validators (e.g. `CreateProjectCommandValidator`) are registered via assembly scanning. `ValidationBehavior<TRequest, TResponse>` runs before every MediatR handler and throws `ValidationException` on failure.

### 4. Authentication & Security

- JWT settings in `appsettings.json` under `JwtSettings` (Secret, Issuer, Audience, ExpiryMinutes)
- `AuthService` issues tokens with `sub`, `email`, `firstName`, and `lastName` claims
- `CurrentUserService` reads the `sub` claim (with fallback to `ClaimTypes.NameIdentifier`)
- Endpoint security via `[Authorize]` and `[AllowAnonymous]` attributes

### 5. Global Error Handling

`GlobalExceptionHandler` (`IExceptionHandler`) maps exceptions to RFC 7807 Problem Details:

| Exception | HTTP Status |
|-----------|-------------|
| `ValidationException` (FluentValidation) | 400 — field-level errors |
| `BadRequestException` | 400 — single detail message |
| `UnauthorizedException` | 401 |

Controllers do not catch exceptions locally; errors flow to the global handler.

### 6. Database Initialisation

On startup, `Program.cs` calls `await app.InitialiseDatabaseAsync()` which:

1. Applies pending EF Core migrations
2. Seeds a default admin user (if missing)
3. Seeds a sample portfolio project (if the Projects table is empty)

**Default seed credentials (Development only):**

- Email: `admin@portfolio.com`
- Password: `Admin123!`

### 7. API Documentation

Scalar UI is available in Development at:

- **HTTPS:** `https://localhost:7118/scalar/v1`
- **OpenAPI JSON:** `https://localhost:7118/openapi/v1.json`

JWT Bearer security scheme metadata is configured so Scalar can send authenticated requests.

---

## Domain Model

### ApplicationUser

Extends `IdentityUser` with `FirstName`, `LastName`, and `CreatedAt`.

### Project

Core portfolio entity with title, description, URLs (`ImageUrl`, `DemoUrl`, `ProjectUrl`, `GithubUrl`), featured flag, display order, and a collection of `Skill` entities.

### Skill

Belongs to a project. Has name, category (e.g. "Backend", "Frontend"), and proficiency percentage.

### WorkExperience

Company, position, description, start/end dates, and current-role flag. Entity and EF configuration exist; API endpoints are not yet implemented.

### BaseAuditableEntity

All domain entities inherit audit fields (`CreatedAt`, `CreatedBy`, `LastModifiedAt`, `LastModifiedBy`). `ApplicationDbContext.SaveChangesAsync` sets these automatically via `ICurrentUserService`.

---

## API Endpoints

| Area | Method | Endpoint | Access | Status |
|------|--------|----------|--------|--------|
| Auth | POST | `/api/Auth/register` | Public | Active |
| Auth | POST | `/api/Auth/login` | Public | Active |
| Auth | GET | `/api/Auth/me` | `[Authorize]` | Active |
| Projects | GET | `/api/projects` | Public | Active |
| Projects | GET | `/api/projects/{id}` | Public | Active |
| Projects | POST | `/api/projects` | `[Authorize]` | Active |

### Query Parameters

- `GET /api/projects?isFeatured=true` — filter to featured projects only

---

## Configuration

### Connection String (Development)

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PortfolioAppDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

### JWT Settings

```json
"JwtSettings": {
  "Secret": "<32+ byte secret>",
  "Issuer": "PortfolioApp",
  "Audience": "PortfolioAppUsers",
  "ExpiryMinutes": 60
}
```

> **Note:** Move secrets to User Secrets or environment variables for production deployments.

### Local URLs

| Profile | URL |
|---------|-----|
| HTTPS | `https://localhost:7118` |
| HTTP | `http://localhost:5181` |

---

## Current Implementation State

- [x] Solution layers separated with correct dependency direction
- [x] EF Core DbContext, migrations, and entity configurations
- [x] ASP.NET Core Identity + JWT authentication (register, login, bearer validation)
- [x] Single `ApplicationUser` in Domain layer (no duplicates)
- [x] MediatR CQRS handlers connected to controllers
- [x] FluentValidation pipeline behavior for commands
- [x] Global exception handler with RFC 7807 Problem Details
- [x] Database migration and seeding on startup
- [x] Scalar OpenAPI documentation in Development
- [x] Audit fields on domain entities

---

## Roadmap / Next Steps

- [ ] **Skills & WorkExperience API** — CQRS handlers and controllers for remaining domain entities
- [ ] **Auth request validation** — FluentValidation rules for `RegisterRequest` and `LoginRequest`
- [ ] **Update / Delete projects** — `UpdateProjectCommand`, `DeleteProjectCommand`
- [ ] **Catch-all exception handler** — handle unhandled exceptions with a generic 500 Problem Details response
- [ ] **Integration tests** — `WebApplicationFactory` + xUnit test project
- [ ] **Package version alignment** — bump `Microsoft.AspNetCore.OpenApi` from 9.x to 10.x
- [ ] **Production secrets** — User Secrets / Azure Key Vault / environment variables for JWT secret and connection string

---

## Running Locally

```powershell
# From repository root
dotnet build JasonCabugPortfolioApp.slnx
dotnet run --project PortfolioApp.Api
```

Then open Scalar at `https://localhost:7118/scalar/v1` or test with the HTTP file in the API project.

### Quick Smoke Test

1. `GET /api/projects` — returns seeded sample project
2. `POST /api/Auth/login` with `admin@portfolio.com` / `Admin123!` — returns JWT
3. `GET /api/Auth/me` with `Authorization: Bearer <token>` — returns user id and email
4. `POST /api/projects` with Bearer token and valid body — creates a project (201)
5. `POST /api/projects` with missing title — returns 400 validation Problem Details

---

## Repository

**Solution file:** `JasonCabugPortfolioApp.slnx`

**Projects:**

| Folder | Project Name |
|--------|-------------|
| `PortfolioApp/` | PortfolioApp.Domain |
| `PortfolioApp.Application/` | PortfolioApp.Application |
| `PortfolioApp.Infrastructure/` | PortfolioApp.Infrastructure |
| `PortfolioApp.Api/` | PortfolioApp.Api |
