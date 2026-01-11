# Architecture

Finament follows Clean Architecture principles, separating concerns into distinct layers.

## Layer Overview

```
┌─────────────────────────────────────────┐
│              Finament.Api               │  ← Entry point, HTTP layer
├─────────────────────────────────────────┤
│          Finament.Application           │  ← Business logic, services
├─────────────────────────────────────────┤
│            Finament.Domain              │  ← Entities, core models
├─────────────────────────────────────────┤
│         Finament.Infrastructure         │  ← Database, external services
└─────────────────────────────────────────┘
```

## Projects

### Finament.Domain

The core layer containing entity definitions. No dependencies on other projects.

**Entities:**
- `User` - User account (id, name, email, passwordHash)
- `Category` - Expense category with monthly budget limit and color
- `Expense` - Individual expense with amount, date, category, and optional tag
- `Setting` - User preferences (currency, billing cycle start day)

### Finament.Application

Business logic layer containing services, DTOs, and interfaces.

**Structure:**
```
Application/
├── DTOs/
│   ├── Auth/           # Login request/response
│   ├── Categories/     # Category CRUD DTOs
│   ├── Expenses/       # Expense CRUD DTOs
│   └── Settings/       # Settings DTOs
├── Exceptions/         # Business exceptions
├── Infrastructure/     # DB context interface
├── Interfaces/         # Service interfaces
├── Mapping/            # Entity ↔ DTO mappers
├── Security/           # Password hashing, JWT options
└── Services/           # Business logic implementations
```

**Key Services:**
- `AuthService` - Login, token generation
- `CategoryService` - Category CRUD with validation
- `ExpenseService` - Expense CRUD with category validation
- `SettingService` - User settings upsert
- `UserService` - User management

### Finament.Infrastructure

Data access layer with EF Core implementation.

**Contents:**
- `FinamentDbContext` - EF Core DbContext
- `Migrations/` - Database migrations

### Finament.Api

HTTP layer with controllers and middleware.

**Contents:**
- `Controllers/` - REST API endpoints
- `Middleware/` - Exception handling
- `Security/` - JWT token service, user context

## Data Flow

```
HTTP Request
    ↓
Controller (validates route, extracts user from JWT)
    ↓
Service (business logic, validation)
    ↓
DbContext (EF Core queries)
    ↓
PostgreSQL
```

## Authentication

JWT-based authentication flow:

1. User calls `POST /api/auth/login` with email/password
2. `AuthService` validates credentials, generates JWT
3. Client includes JWT in `Authorization: Bearer <token>` header
4. `UserContext` extracts user ID from JWT claims
5. Services use `userId` to scope queries

## Key Design Decisions

**Manual Mapping**: Uses static mapping classes instead of AutoMapper for simplicity and explicit control.

**Scoped Services**: All services are registered as scoped (per-request) for proper DbContext lifecycle.

**Cascade Deletes**: Deleting a category removes all associated expenses (handled in service layer).

**Computed Fields**: `ExpenseCount` and `TotalSpent` on categories are computed at query time, not stored.
