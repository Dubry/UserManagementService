# User Management API

This project is an ASP.NET Core Web API for managing users, including secure endpoints with API Key authentication and rate-limiting. It includes integration tests and Swagger/OpenAPI support.

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)  
- SQL Server (for production database)  
- Optional: [Postman](https://www.postman.com/) or any HTTP client for testing APIs

---

## Getting Started

### 1. Configure the Database
Open appsettings.json and set the connection string for SQL Server:
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=UserManagementDb;Trusted_Connection=True;"
}
```
Apply EF Core migrations:
```bash
dotnet ef database update
```

### 2. Run the Application
```bash
dotnet run
```

## API Key Authentication
All `/api/users` endpoints are protected by an API key.

The API key must be sent in the request header:

X-API-KEY: `<your-api-key>`
### Example Seeded API Keys

| Client      | API Key        |
|-------------|----------------|
| Swagger     | swagger-dev-key|
| Integration | test-api-key   |

Rate Limiting

POST /api/users/{id}/validate-password is rate-limited to 5 requests per minute per client.

Requests exceeding the limit will receive HTTP 429 Too Many Requests.