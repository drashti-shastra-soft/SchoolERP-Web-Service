# SchoolERP Web API

SchoolERP Web API is a RESTful ASP.NET Core Web API powering the SchoolERP platform. It provides secure endpoints for admissions, students, classes, attendance, exams, results, fees, payroll, transport, and notices. The API uses JWT authentication, role-based access control, and structured logging, and is built on a clean layered architecture with EF Core. It is designed as a multi-tenant system, serving multiple companies/schools with isolated data and academic sessions, and it powers the SchoolERP Web Console.

## Features

- **Multi-tenant architecture** — supports multiple companies/schools in a single instance with strict data isolation.
- **Session management** — academic sessions (e.g., 2024-25) scoped per company/school.
- **Authentication & Authorization** — JWT-based authentication with role-based access control (SuperAdmin, Admin, Teacher, Parent, Student).
- **Admissions** — manage admission inquiries, applications, and enrollment endpoints.
- **Student management** — student records, profiles, documents, and history endpoints.
- **Staff management** — teacher and staff records, roles, and assignments.
- **Class & section management** — classes, sections, subjects, and teacher assignments.
- **Attendance** — daily attendance tracking for students and staff.
- **Exams & results** — schedule exams, record marks, and publish results.
- **Fees management** — fee structures, invoices, payments, and receipts.
- **Staff payroll** — salary structures, payroll processing, and payslips.
- **Transport** — routes, vehicles, drivers, and student transport assignments.
- **Notices & communication** — share notices and announcements with parents and staff.
- **AMC & Company Management** — manage company details, subscriptions, and Annual Maintenance Contracts (SuperAdmin only).
- **Logging and auditing** — structured request/response logging and audit trails.
- **API documentation** — Swagger/OpenAPI for exploring and testing endpoints.

> Feature descriptions are based on the controllers, domain models, services, and repositories present in this repository. Actual behavior may depend on the connected SQL database and configuration.

## Technology stack

- C# and ASP.NET Core `<6 / 7 / 8>`
- Clean layered architecture (API, Application, Domain, Infrastructure)
- **Entity Framework Core** for standard CRUD operations and querying.
- **ADO.NET and Stored Procedures** for complex queries, bulk operations, and reporting (no Lambda expressions for complex logic).
- **Database-First approach** — models and contexts are generated from the existing SQL Server schema.
- SQL Server database
- JWT authentication with role-based authorization
- Structured logging via `<Serilog / built-in ILogger>`
- Swagger / OpenAPI for API documentation
- Visual Studio solution (`SchoolERP.API.sln`)

## Solution structure

```text
SchoolERP/
├── SchoolERP.API.sln
├── SchoolERP.API/                        # ASP.NET Core Web API host
│   ├── Controllers/                      # API endpoints
│   ├── Middleware/                       # Exception handling, logging, tenant resolution
│   ├── Filters/                          # Action filters and validation
│   ├── Extensions/                       # Service registration extensions
│   ├── Config/                           # Environment-specific app settings
│   └── Program.cs
├── SchoolERP.Application/                # Use cases, DTOs, interfaces, validators
├── SchoolERP.Domain/                     # Entities, value objects, enums
└── SchoolERP.Infrastructure/             # EF Core DbContext, repositories, ADO.NET, external services