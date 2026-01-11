# Finament API

A personal finance management API built with .NET 8 and Clean Architecture.

## Overview

Finament is a backend service for tracking expenses and managing budgets. Users can create spending categories with monthly limits, log expenses, and configure personal settings like currency and billing cycle.

## Tech Stack

- **.NET 8** - Web API framework
- **PostgreSQL** - Database (via Supabase)
- **Entity Framework Core** - ORM
- **JWT** - Authentication
- **Swagger/OpenAPI** - API documentation

## Project Structure

```
Finament/
├── Finament.Api/            # Controllers, middleware, Program.cs
├── Finament.Application/    # Services, DTOs, business logic
├── Finament.Domain/         # Entities
├── Finament.Infrastructure/ # Database context, migrations
└── docs/                    # Documentation
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- PostgreSQL database

### Configuration

Create an `appsettings.Development.json` in `Finament.Api/`:

```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Database=finament;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Key": "your-secret-key-min-32-characters",
    "Issuer": "finament",
    "Audience": "finament-client",
    "ExpirationMinutes": 60
  }
}
```

### Run

```bash
cd Finament.Api
dotnet run
```

The API will be available at `https://localhost:5001` (or the port configured).

Swagger UI is available at `/swagger` in development mode.

### Database Migrations

```bash
cd Finament.Api
dotnet ef database update --project ../Finament.Infrastructure
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/auth/login` | Login and get JWT token | No |
| GET | `/api/auth/health` | Health check | No |
| GET | `/api/categories` | Get all categories | Yes |
| POST | `/api/categories` | Create category | Yes |
| PUT | `/api/categories/{id}` | Update category | Yes |
| DELETE | `/api/categories/{id}` | Delete category | Yes |
| GET | `/api/expenses` | Get all expenses | Yes |
| POST | `/api/expenses` | Create expense | Yes |
| PUT | `/api/expenses/{id}` | Update expense | Yes |
| DELETE | `/api/expenses/{id}` | Delete expense | Yes |
| GET | `/api/settings` | Get user settings | Yes |
| PUT | `/api/settings` | Update settings | Yes |

See [docs/api.md](docs/api.md) for detailed API documentation.

## Documentation

- [Architecture](docs/architecture.md) - Project structure and design decisions
- [API Reference](docs/api.md) - Detailed endpoint documentation

## License

Private
