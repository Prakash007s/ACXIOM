# AcxiomCRM (ASP.NET Core 8 MVC + Identity + EF Core)
1. `dotnet restore`
2. `dotnet tool install -g dotnet-ef` then `dotnet ef migrations add Initial` (app auto-migrates on start)
3. `dotnet run` -> https://localhost:5001
Seeded admin: admin@acxiom.local / Admin@12345 (change in appsettings / user-secrets). Self-registered users get SalesExecutive.
API: GET/POST /api/customers, GET/PUT/DELETE /api/customers/{id} (cookie auth; 401/403/404/409).
