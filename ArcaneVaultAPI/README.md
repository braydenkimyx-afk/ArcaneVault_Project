# ArcaneVault — Collection Management System

A two-project ASP.NET Core solution for managing collectible items, built with **Razor Pages** + **Web API** + **EF Core** + **SQLite**.

## Projects

| Project | Type | Port | Description |
|---------|------|------|-------------|
| `ArcaneVaultAPI` | Web API | 5001 | REST API with EF Core + SQLite, Scalar OpenAPI docs |
| `ArcaneVault.Web` | Razor Pages | 5000 | Frontend with Bootstrap 5, Chart.js, cookie auth |

## Architecture

```
ArcaneVault.Web  →  HttpClient (IApiClient)  →  ArcaneVaultAPI  →  EF Core  →  SQLite
```

The Web project **never touches the database directly**. All data access goes through the typed `IApiClient` service, which makes HTTP calls to the API.

## Getting Started

### Prerequisites
- .NET 10 SDK

### Run the API
```bash
cd ArcaneVaultAPI
dotnet run
```
The database is automatically created and seeded on first startup via `Database.Migrate()`.

### Run the Web App (in a separate terminal)
```bash
cd ArcaneVault.Web
dotnet run
```

### Test Accounts
| Username | Password | Role |
|----------|----------|------|
| `admin` | `Admin@123` | Staff |
| `member1` | `Member@123` | Member |

## Features

### API Endpoints
- **Account**: Register, Login, GET/PUT/DELETE users, password reset, lockout
- **Categories**: Full CRUD with 409 conflict protection on delete
- **Collection Items**: Full CRUD, soft delete, search
- **Analytics**: Dashboard stats, low-stock alerts, CSV export

### Web Application
- **Authentication**: Cookie-based with PBKDF2 hashed passwords
- **Role-Based Access**: Staff sees all data + category management; Members see their own items
- **Dashboard**: KPI cards, Chart.js visualizations (doughnut, bar, grouped-bar), low-stock alerts
- **Security**: Password strength meter, account lockout after 5 failed attempts, time-limited reset tokens
- **UX**: Toast notifications, confirmation modals, search, responsive dark theme

## Technology Stack
- ASP.NET Core 10 (Web API + Razor Pages)
- Entity Framework Core 10 + SQLite
- Bootstrap 5.3 + Bootstrap Icons
- Chart.js 4
- jQuery Validation Unobtrusive
- Scalar (OpenAPI documentation)
