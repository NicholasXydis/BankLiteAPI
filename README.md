# BankLiteAPI

A production-ready banking REST API built with ASP.NET Core 8 and Clean Architecture.  
*Une API REST bancaire prête pour la production, construite avec ASP.NET Core 8 et Clean Architecture.*

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=flat&logo=microsoftsqlserver)
![JWT](https://img.shields.io/badge/JWT-Auth-000000?style=flat&logo=jsonwebtokens)
![BCrypt](https://img.shields.io/badge/BCrypt-Password_Hashing-green?style=flat)
![xUnit](https://img.shields.io/badge/xUnit-Tests-512BD4?style=flat)
![Moq](https://img.shields.io/badge/Moq-Mocking-blue?style=flat)
![FluentValidation](https://img.shields.io/badge/FluentValidation-Input_Validation-orange?style=flat)
![Serilog](https://img.shields.io/badge/Serilog-Logging-004880?style=flat)
![GitHub Actions](https://img.shields.io/badge/CI/CD-GitHub_Actions-2088FF?style=flat&logo=githubactions)
![Rate Limiting](https://img.shields.io/badge/Rate_Limiting-Enabled-red?style=flat)
![Health Check](https://img.shields.io/badge/Health_Check-Enabled-brightgreen?style=flat)
![EF Core](https://img.shields.io/badge/EF_Core-8.0-512BD4?style=flat&logo=dotnet)
![Swagger](https://img.shields.io/badge/Swagger-UI-85EA2D?style=flat&logo=swagger)

---

## Overview / Aperçu

BankLiteAPI is a fully featured banking REST API demonstrating production-level backend development practices. It supports user authentication, account management, financial transactions, and audit logging — all built with a strict Clean Architecture pattern.

*BankLiteAPI est une API REST bancaire complète démontrant des pratiques de développement backend de niveau production. Elle supporte l'authentification des utilisateurs, la gestion des comptes, les transactions financières et la journalisation des audits — le tout construit avec une Clean Architecture stricte.*

---

## Features / Fonctionnalités

- JWT authentication with BCrypt password hashing — passwords never stored in plain text
- User registration and login with email normalization to lowercase
- Chequing and savings account creation with auto-generated account numbers
- Deposit and withdrawal with full transaction receipt returned to client
- Atomic transfers between accounts using Unit of Work — full rollback on failure
- Transaction descriptions stored on every deposit, withdrawal and transfer
- Paginated transaction history ordered by newest first
- FluentValidation on all endpoints — input sanitization before hitting the service layer
- Repository pattern — clean data access abstraction across all entities
- Unit of Work pattern — atomic database operations with commit and rollback
- Rate limiting — 5 attempts/min on login for brute force protection, 30/min globally
- Health check endpoint at `/health` for production monitoring
- Audit logging on all financial operations and authentication events
- Structured logging with Serilog to console and daily rolling file
- Global exception middleware — 400 for business errors, 500 for server errors
- 201 Created returned on account creation following REST conventions
- Swagger UI with JWT authorization button for easy API testing
- Seed data for immediate local testing
- CI/CD pipeline with GitHub Actions — runs all 6 tests on every push

---

## Architecture

BankLiteAPI follows Clean Architecture with strict separation of concerns across four layers:

**Domain** — Core entities (`User`, `Account`, `Transaction`, `AuditLog`) and repository/service interfaces. Zero dependencies on other layers.

**Application** — Business logic, DTOs, FluentValidation validators, and service implementations (`AuthService`, `AccountService`, `TransactionService`).

**Infrastructure** — EF Core repository implementations, `BankLiteDbContext`, `UnitOfWork`, and `SeedData`.

**API** — ASP.NET Core controllers, `ExceptionMiddleware`, and `Program.cs` configuration.

*BankLiteAPI suit une Clean Architecture avec une séparation stricte des responsabilités sur quatre couches : Domain, Application, Infrastructure et API.*

---

## Tech Stack / Technologies

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 |
| Database | SQL Server + Entity Framework Core 8 |
| Authentication | JWT Bearer + BCrypt.Net |
| Validation | FluentValidation |
| Testing | xUnit + Moq |
| Logging | Serilog (Console + File) |
| API Documentation | Swagger / Swashbuckle |
| CI/CD | GitHub Actions |
| Security | Rate Limiting + BCrypt + JWT |
| Architecture | Clean Architecture + Repository Pattern + Unit of Work |

---

## Getting Started / Démarrage

### Prerequisites / Prérequis
- .NET 8 SDK
- SQL Server or SQL Server Express

### Setup / Configuration

**1. Clone the repository / Cloner le dépôt**
```bash
git clone https://github.com/NicholasXydis/BankLiteAPI.git
cd BankLiteAPI/BankLiteAPI
```

**2. Configure your settings / Configurez vos paramètres**

Copy `appsettings.example.json` to `appsettings.json` and fill in your values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=BankLiteDB;Trusted_Connection=True;"
  },
  "JwtSettings": {
    "Secret": "YOUR_SECRET_KEY_MIN_32_CHARACTERS",
    "Issuer": "BankLiteAPI",
    "Audience": "BankLiteClient",
    "ExpiryMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**3. Apply migrations / Appliquer les migrations**
```bash
dotnet ef database update --project BankLite.Infrastructure --startup-project BankLite.API
```

**4. Run the API / Lancer l'API**
```bash
dotnet run --project BankLite.API
```

**5. Open Swagger / Ouvrir Swagger**

Navigate to `https://localhost:7205/swagger`

---

## Running Tests / Lancer les tests

```bash
dotnet test
```

6 tests across `AuthServiceTests` and `TransactionServiceTests`:

| Test | Description |
|---|---|
| `RegisterAsync_ShouldThrow_WhenEmailAlreadyExists` | Duplicate email throws |
| `LoginAsync_ShouldThrow_WhenUserNotFound` | Unknown email throws |
| `LoginAsync_ShouldThrow_WhenPasswordIsWrong` | Wrong password throws |
| `DepositAsync_ShouldIncreaseBalance` | Balance increases correctly |
| `WithdrawAsync_ShouldThrow_WhenInsufficientFunds` | Insufficient funds throws |
| `TransferAsync_ShouldMoveMoney_BetweenAccounts` | Money moves atomically |

---

## API Endpoints

### Auth — `/api/auth`
| Method | Endpoint | Description | Auth Required |
|---|---|---|---|
| POST | `/register` | Register a new user | No |
| POST | `/login` | Login and receive JWT token | No |

### Accounts — `/api/account`
| Method | Endpoint | Description | Auth Required |
|---|---|---|---|
| POST | `/create` | Create a chequing or savings account | Yes |
| GET | `/` | Get all accounts for authenticated user | Yes |

### Transactions — `/api/transaction`
| Method | Endpoint | Description | Auth Required |
|---|---|---|---|
| POST | `/deposit` | Deposit funds — returns transaction receipt | Yes |
| POST | `/withdraw` | Withdraw funds — returns transaction receipt | Yes |
| POST | `/transfer` | Atomic transfer between accounts | Yes |
| GET | `/{accountId}?page=1&pageSize=10` | Get paginated transaction history | Yes |

### Health
| Method | Endpoint | Description |
|---|---|---|
| GET | `/health` | Returns API health status |

---

## Seed Data / Données de test

On first run the database is automatically seeded with a test user and two accounts:

| Field | Value |
|---|---|
| Email | test@banklite.com |
| Password | Password123 |
| Chequing Balance | $1,000.00 |
| Savings Balance | $5,000.00 |

---

## Security / Sécurité

- Passwords hashed with BCrypt — never stored in plain text
- JWT tokens expire after 60 minutes
- Login endpoint rate limited to 5 attempts per minute — brute force protection
- All other endpoints rate limited to 30 requests per minute
- Email addresses normalized to lowercase before storage
- Unit of Work with full rollback on transfer failure
- All sensitive configuration stored as environment variables in production
- `appsettings.json` is gitignored — never committed to version control

---

## Project Structure / Structure du projet
```
BankLiteAPI/
├── BankLite.Domain/
│   ├── Entities/          # User, Account, Transaction, AuditLog
│   └── Interfaces/        # Repository and service interfaces
├── BankLite.Application/
│   ├── DTOs/              # Data transfer objects
│   ├── Interfaces/        # Service interfaces
│   ├── Services/          # AuthService, AccountService, TransactionService
│   └── Validators/        # FluentValidation validators
├── BankLite.Infrastructure/
│   ├── Data/              # BankLiteDbContext, UnitOfWork, SeedData
│   ├── Migrations/        # EF Core migrations
│   └── Repositories/      # Repository implementations
├── BankLite.API/
│   ├── Controllers/       # AuthController, AccountController, TransactionController
│   ├── Middleware/        # ExceptionMiddleware
│   └── Program.cs         # App configuration
└── BankLite.Tests/
    └── Services/          # AuthServiceTests, TransactionServiceTests
```

---

## Author / Auteur

**Nicholas Xydis**  
GitHub: [NicholasXydis](https://github.com/NicholasXydis)