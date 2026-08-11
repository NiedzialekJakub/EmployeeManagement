A RESTful backend API built with C# and .NET for managing employee profiles, featuring robust validation Clean Architecture, and a fault-tolerand CSV bulk import tool.


Technology Stack & Justyfication

Framework: .NET 10 (C#) – Chosen for high performance and built-in dependency injection.
Architecture: Clean Architecture & CQRS (MediatR) – Keeps core business domain rules decoupled from HTTP presentation and database frameworks, ensuring testability and long-term maintainability.
Database & ORM: SQLite + Entity Framework Core – Zero-configuration file-based database ideal for local evaluation, paired with EF Core.
Validation: FluentValidation + Pipeline Behaviors – Enforces strict domain validation rules before requests reach command handlers.
CSV Processing: CsvHelper – High-performance stream processing for CSV bulk ingestion.
Testing: xUnit, Moq, EF Core InMemory – Isolated unit testing for validation logic, command handlers, and API controllers.

How to Run Locally
.NET 10 SDK installed on your machine.

Running the application
1. Clone the repository
2. cd EmployeeManagement
3. Run the API: dotnet run --project Api/Api.csproj

Running Unit Tests: dotnet test

API Endpoints
POST /employee Create a new employee
GET /employee/{id} Retrieve details for a single employee
GET /employees List all employees
PUT /employee/{id} Update an existing employee profile
DELETE/employee/{id} Remove an employee record
POST/employees/bulk Bulk import employees from a CSV file

Usage Examples

Delete the employee (DELETE/employee/{id})
http://localhost:5000/api/employee/90f4149c-be3c-485f-8a1e-2d1dbc1b5684

Get one employee (GET /employee/{id})
http://localhost:5000/api/employee/18f449a9-4f98-4a81-b401-43ab50dcc2ab

Edit employee (PUT /employee/{id})
http://localhost:5000/api/employee/e6e166f5-1b21-4d19-8da9-207f07e6dd57
Request Body:
{
        "id": "e6e166f5-1b21-4d19-8da9-207f07e6dd57",
        "name": "Karol Karol Nowak",
        "hireDate": "2025-08-10T13:10:09.1087857",
        "email": "karol.nowak@gmail.com",
        "phoneNo": "+48987651321",
        "profilePicture": "https://example.com/photos/knowak.jpg",
        "status": "active",
        "address": "30 Piotrkowska",
        "state": "Mazowieckie",
        "country": "Poland",
        "city": "Radom",
        "pincode": "10201",
        "createdAt": "2026-08-10T13:10:09.1087868"
    }

Get all employees (GET /employees)
http://localhost:5000/api/employees

Create Employee (POST /employee)
http://localhost:5000/api/employee
Request Body:
{
  "name": "Jan Kowalski",
  "hireDate": "2024-01-15T00:00:00Z",
  "email": "jan.kowalski@example.com",
  "phoneNo": "+48123456789",
  "profilePicture": "[https://example.com/photos/jkowalski.jpg](https://example.com/photos/jkowalski.jpg)",
  "status": "active",
  "address": "10 Marszałkowska",
  "city": "Warsaw",
  "state": "Mazowieckie",
  "country": "Poland",
  "pincode": "00-001"
}

Import from CSV (POST/employees/bulk)
http://localhost:5000/api/employees/bulk
Request Body: CSV file

Additional Business Rules & Validations

1. Strict Status Whitelist: Restricted Status exclusively to 'active' or 'inactive' to prevent invalid strings.
2. Pincode Pattern: Applied regex validation to ensure postal code format validity (^[A-Za-z0-9\s-]{3,10}$) before persistence.
3. Valid Profile Picture URL: Enforced URI format checking (Uri.TryCreate) on ProfilePicture to avoid broken image links on client applications.
4. Full Name Composition Rule: Required Name to consist of at least two words (first and last name) with length constraints (3–100 characters) to prevent incomplete records.

Key Architectural & Design Decisions

1. Clean Architecture & CQRS (MediatR): Separated the solution into API, Application, Domain, Persistence, and Tests projects to keep business logic thin, testable, and independent of external frameworks.
2. Guid Primary Keys: Used Guid for employee identifiers to prevent ID enumeration/guessing vulnerabilities and allow ID generation prior to persistence.
3. Pipeline Validation (FluentValidation): Integrated a generic MediatR ValidationBehavior to validate incoming requests before hitting handlers, ensuring invalid requests fail fast.
4. Global Exception Middleware: Intercepted validation exceptions and unhandled errors globally, transforming them into RFC-compliant HTTP 400 and HTTP 500 JSON responses.
5. Partial Success Bulk Import: Implemented a fault-tolerant CSV processing engine that imports all valid rows while returning a structured error audit report for invalid entries, rather than discarding the entire transaction.
6. String Data Types for Postal Codes & Telephony: Modeled Pincode and PhoneNo as strings to preserve leading zeros and support international characters (+, -, spaces).
7. Startup Seeding: Created a DbInitializer class to apply EF Core migrations and seed default test data on server startup.

AI Tool Usage
Gemini for rapid boilerplate generation and initial xUnit test scaffolding

What was Kept, Changed or Reject
Rejected AI Bulk Transaction Rollback: AI initially suggested wrapping the CSV import in a strict transaction that rolls back all records if a single row fails. I rejected this in favor of a Partial Success Pattern with detailed audit logs.

What I'd Do Differently With More Time
Structured Logging: Implement Serilog with centralized logging targets for better operational observability
