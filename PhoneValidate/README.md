# PhoneValidate API — .NET Clean Architecture Template

[![CI](https://github.com/rodrigorf/phone-validate/actions/workflows/ci.yml/badge.svg)](https://github.com/rodrigorf/phone-validate/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A production-style REST API template built with **.NET 10**, **Clean Architecture**, **Entity Framework Core**, **JWT authentication**, and a full **Docker Compose** setup. Clone it, run one command, and you have a working API with a database, auth, Swagger, health checks, and tests.

> Ideal as a starting point for new services or as a reference for how to wire up a clean, layered .NET API.

---

## ✨ Features

- **Clean Architecture** — Domain / Application / Infrastructure / Api separation
- **Entity Framework Core** + **SQL Server** with code-first **migrations**
- **Generic repository pattern** (`IBaseRepository<T>`)
- **JWT authentication** (Bearer tokens)
- **Swagger / OpenAPI** UI
- **Health checks** (`/healthystatus`) with database probe
- **Result pattern** for clean success/error flows (no exceptions for control flow)
- **xUnit** unit tests + **Reqnroll** BDD end-to-end tests
- **Docker Compose** — API + SQL Server, one command to run
- **Multi-stage Dockerfile** for small, optimized images

---

## 🧱 Project Structure

```
PhoneValidate/
├── PhoneValidate/                      # Api    — Controllers, Program.cs, Extensions
├── PhoneValidate.Application.Service/  # Application — AppServices, DTOs, Mapping
├── PhoneValidate.Domain.Service/       # Domain — Entities, Domain Services, Interfaces
├── PhoneValidate.Infra.Data/           # Infrastructure — DbContext, Repositories, Migrations
├── PhoneValidate.Unit.Tests/           # xUnit unit tests
├── PhoneValidate.E2E.Tests/            # Reqnroll BDD tests
├── docker-compose.yml                  # API + SQL Server
├── .env.example                        # Environment variable template
└── PhoneValidate.sln
```

The dependency flow points inward: `Api → Application → Domain ← Infrastructure`. The Domain layer has no dependencies on other layers.

---

## 🚀 Quick Start

**Prerequisites:** [Docker](https://www.docker.com/) (with Docker Compose).

```bash
# 1. Create your environment file
cp .env.example .env

# 2. Run everything (API + database)
docker compose up --build
```

That's it. The API and SQL Server start together, the database schema is created automatically via EF Core migrations, and the API is ready.

| Service | URL |
|---------|-----|
| API     | http://localhost:5000 |
| Swagger | http://localhost:5000/swagger |
| Health  | http://localhost:5000/healthystatus |

To stop: `docker compose down` (add `-v` to also wipe the database volume).

---

## 🔑 Authentication

All `Recipient` endpoints require a JWT Bearer token. Get one from the auth endpoint:

```bash
curl -X POST http://localhost:5000/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{"username":"test","password":"test"}'
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiI...",
  "expiration": "2026-06-01T02:32:55Z"
}
```

> **Demo credentials.** `test` / `test` are hardcoded in `AuthController` purely so you can try the API immediately. Replace this with a real user store / identity provider before going to production.

Tip — capture the token into a shell variable:
```bash
TOKEN=$(curl -s -X POST http://localhost:5000/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{"username":"test","password":"test"}' | grep -o '"token":"[^"]*"' | cut -d'"' -f4)
```

---

## 📦 Usage Examples

### Create a recipient
```bash
curl -X POST http://localhost:5000/Recipient \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"phoneNumber":"+55 11 91234-5678"}'
```
```json
{ "id": "afd0659e-...", "updatedAt": "2026-06-01T01:33:04Z", "phoneNumber": "+5511912345678" }
```
> Phone numbers are normalized on save (whitespace and separators stripped, leading `+` enforced).

### Get a recipient by phone number
```bash
curl "http://localhost:5000/Recipient?phoneNumber=%2B5511912345678" \
  -H "Authorization: Bearer $TOKEN"
```

### Update a recipient
```bash
curl -X PUT http://localhost:5000/Recipient/{id} \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"phoneNumber":"+55 21 99999-8888"}'
```

### Delete a recipient
```bash
curl -X DELETE http://localhost:5000/Recipient/{id} \
  -H "Authorization: Bearer $TOKEN"
```

| Method | Route | Description | Auth |
|--------|-------|-------------|------|
| POST   | `/api/auth/token` | Issue a JWT | ❌ |
| GET    | `/Recipient?phoneNumber=` | Get by phone number | ✅ |
| POST   | `/Recipient` | Create | ✅ |
| PUT    | `/Recipient/{id}` | Update | ✅ |
| DELETE | `/Recipient/{id}` | Delete | ✅ |
| GET    | `/healthystatus` | Health check | ❌ |

---

## 🧪 Running Tests

```bash
dotnet test
```

Runs unit tests (domain service logic, validation, phone normalization) and the BDD end-to-end scenarios.

---

## 🛠️ Running Locally (without Docker)

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download) and a reachable SQL Server instance.

1. Update the `ConnectionStrings:Default` value in `PhoneValidate/appsettings.json` (or set it via environment variable / user secrets).
2. Run the API:
   ```bash
   dotnet run --project PhoneValidate
   ```
Migrations are applied automatically on startup.

---

## ⚙️ Configuration

Configured via environment variables (Docker) or `appsettings.json` (local). Key settings:

| Variable | Description |
|----------|-------------|
| `ConnectionStrings__Default` | SQL Server connection string |
| `Jwt__Key` | JWT signing key (min 32 chars) |
| `Jwt__Issuer` / `Jwt__Audience` | Token issuer / audience |
| `Jwt__ExpiryMinutes` | Token lifetime (default 60) |

When using Docker, set `DB_PASSWORD` and `JWT_KEY` in your `.env` file (see `.env.example`).

---

## 📸 Screenshots

> _Add a screenshot of the Swagger UI here, e.g._ `docs/swagger.png`
>
> `![Swagger UI](docs/swagger.png)`

---

## 🤝 Contributing

Contributions are welcome. Fork the repo, create a feature branch, and open a pull request. Please make sure `dotnet test` passes before submitting.

---

## 📄 License

MIT — see [LICENSE](LICENSE). You are free to use this template in personal and commercial projects.
