# AcxiomCRM — Customer Relationship Management System

A web-based Customer Relationship Management (CRM) application built with **ASP.NET Core MVC, .NET 10, ASP.NET Core Identity, and Entity Framework Core**. AcxiomCRM provides a structured platform for managing customer information, tracking sales leads and opportunities, scheduling follow-ups, recording activities, and maintaining audit logs.

**Repository:** [Prakash007s/ACXIOM](https://github.com/Prakash007s/ACXIOM)

## Overview

AcxiomCRM is designed to organise customer relationships and sales activities in one application. It combines an MVC-based web interface with database persistence, user authentication, role-based authorisation, and auditing of important operations.

## Key Features

- **Customer Management:** Maintain customer details, including contact information, company, address, and status.
- **Lead Management:** Track prospective customers, lead sources, expected values, assignments, and lead status.
- **Sales Opportunities:** Manage sales opportunities, expected revenue, probability, pipeline stages, and expected closing dates.
- **Follow-up Management:** Record planned customer or lead follow-ups, dates, types, remarks, and status.
- **Activity Tracking:** Maintain records of sales-related activities and interactions.
- **Audit Logging:** Record significant changes with information such as the user, action, affected record, and timestamp.
- **Authentication:** Manage user accounts using ASP.NET Core Identity.
- **Role-Based Access Control:** Support `Admin`, `Manager`, and `SalesExecutive` roles.
- **Database Migrations:** Apply pending Entity Framework Core migrations automatically during application startup.
- **Input Protection:** Use ASP.NET Core MVC antiforgery validation and server-side validation where configured.

## Technology Stack

| Technology | Purpose |
|---|---|
| C# | Application development |
| .NET 10 | Target framework |
| ASP.NET Core MVC | Web application architecture |
| ASP.NET Core Identity | User authentication and account management |
| Entity Framework Core 10 | ORM and database access |
| SQL Server LocalDB | Local development database |
| Razor Views | Server-rendered user interface |
| Git and GitHub | Source control and project hosting |

## Project Structure

```text
ACXIOM/
├── Controllers/     # MVC request handling
├── Data/            # Application database context
├── Migrations/      # Entity Framework Core migrations
├── Models/          # Domain entities and role definitions
├── Services/        # Application services, including auditing
├── ViewModels/      # Data models for MVC views
├── Views/           # Razor user interface
├── wwwroot/         # Static assets, including CSS
├── Program.cs       # Application configuration and startup
├── appsettings.json # Connection string and application settings
├── AcxiomCRM.csproj # Project and package configuration
└── README.md
```

## Prerequisites

Before running the application, install:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
- [Git](https://git-scm.com/)
- Visual Studio or Visual Studio Code (optional, for development)

The configured LocalDB connection is intended for Windows development. For another database environment, configure a compatible SQL Server connection string.

## Installation and Setup

### 1. Clone the repository

```bash
git clone https://github.com/Prakash007s/ACXIOM.git
cd ACXIOM
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Configure the database

The application reads its database connection string from the `Default` key under `ConnectionStrings`.

The current development configuration uses SQL Server LocalDB:

```json
{
  "ConnectionStrings": {
    "Default": "Server=(localdb)\\MSSQLLocalDB;Database=AcxiomCRM;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Ensure LocalDB is installed and available on your machine.

### 4. Configure administrator credentials

The application seeds an administrator account using the `Seed:AdminEmail` and `Seed:AdminPassword` settings.

For local development, configure the credentials through .NET User Secrets rather than storing a real password in a committed settings file:

```bash
dotnet user-secrets init
dotnet user-secrets set "Seed:AdminEmail" "admin@acxiom.local"
dotnet user-secrets set "Seed:AdminPassword" "YOUR-STRONG-LOCAL-PASSWORD"
```

Replace the placeholder with a strong password that satisfies the application's configured password requirements.

**Security warning:** The repository's current `appsettings.json` contains a default administrator password. Rotate that password, remove the hard-coded secret from tracked configuration, and review Git history if the credential has already been published. User Secrets override the corresponding settings locally but do not remove a secret from Git history.

### 5. Apply database migrations

The application calls Entity Framework Core's migration mechanism during startup. To apply migrations manually before starting the application, install the compatible EF Core CLI tool:

```bash
dotnet tool install --global dotnet-ef --version 10.0.12
```

If it is already installed, update it:

```bash
dotnet tool update --global dotnet-ef --version 10.0.12
```

Then apply the existing migrations:

```bash
dotnet ef database update
```

Do not create a duplicate `Initial` migration if the repository already contains migration files.

### 6. Run the application

```bash
dotnet run
```

Open the HTTPS address shown in the terminal. The actual URL and port depend on the application's launch configuration.

If port 5000 is occupied, you can use an alternative HTTP port for local development:

```bash
dotnet run --urls "http://localhost:5280"
```

Then open [http://localhost:5280](http://localhost:5280).

## User Roles and Access Control

The application defines the following roles:

| Role | Intended purpose |
|---|---|
| `Admin` | Administrative access |
| `Manager` | Broader access to customer records and management operations |
| `SalesExecutive` | Sales-related access, including customer records within the applicable ownership scope |

The customer controller restricts records by creator for ordinary users, while administrators and managers can access a broader set of customer records. Customer deletion is implemented as a status change to `Inactive` and is restricted to administrators and managers.

Actual permissions depend on the authorisation attributes and checks implemented by each controller.

## Data Model

The application's domain entities include:

| Entity | Description |
|---|---|
| `AppUser` | Application user account and profile |
| `Customer` | Customer contact and company information |
| `Lead` | Prospective customer and qualification details |
| `Opportunity` | Sales opportunity, value, stage, and probability |
| `FollowUp` | Planned follow-up actions and remarks |
| `Activity` | Recorded sales and customer activities |
| `AuditLog` | Records of significant application operations |

The database context also defines unique indexes for customer email addresses and phone numbers.

## API and Routing

The current customer controller is an **MVC controller** that serves customer-management views and handles form submissions. Its routes are generated through the default MVC route configuration.

A dedicated REST API with routes such as `GET /api/customers` and `POST /api/customers` should only be documented as available after confirming that corresponding API controller actions exist in the repository. Do not assume that MVC form routes automatically provide REST endpoints.

## Security Considerations

- Use strong administrator credentials and keep secrets outside source control.
- Enforce authentication and role-based authorisation for protected operations.
- Retain antiforgery validation for applicable MVC form submissions.
- Use HTTPS in environments where credentials or customer data are transmitted.
- Review access permissions before deploying to a shared or production environment.
- Back up the database before applying destructive schema or data changes.

## Future Enhancements

Potential future improvements include:

- REST API endpoints with OpenAPI documentation.
- Automated unit and integration testing.
- Advanced lead and opportunity reporting.
- Dashboard analytics and sales performance metrics.
- Pagination and expanded search and filtering.
- Deployment configuration for a production SQL Server instance.

These are proposed enhancements and should not be interpreted as existing functionality unless implemented.

## Author

**Prakash Pal**

- GitHub: [@Prakash007s](https://github.com/Prakash007s)
- Project Repository: [ACXIOM](https://github.com/Prakash007s/ACXIOM)

## License

No licence is currently specified in this README. Add a `LICENSE` file if you intend to define how others may use, modify, or distribute this project.
