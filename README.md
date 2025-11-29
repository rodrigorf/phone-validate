# PhoneValidate API
REST API developed in .NET 10 with JWT authentication, following layered architecture (IDesign).

## 🚀 Tecnologies
- **.NET 10**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server**
- **JWT Authentication**
- **Swagger/OpenAPI**
- **Docker**
- **xUnit + SpecFlow (BDD)**

## 📦 Running the Application with Docker
- Run the following command to start the API:
```docker-compose up --build
```
A API estará disponível em:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger**: http://localhost:5000/swagger
- SQL Server (porta 1433)
- PhoneValidation API (porta 50000)

## 🔑 Authentication

### Get Token JWT

POST /api/auth/token Content-Type: application/json
{ "username": "test", "password": "test" }

{ "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...", "expiration": "2024-01-01T12:00:00Z" }
