using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Employees.Commands;

public class BulkImportResult
{
    public int ImportedCount { get; set; }
    public int SkippedCount { get; set; }
    public List<BulkImportError> Errors { get; set; } = [];
}

public class BulkImportError
{
    public int RowNumber { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

public class BulkImportEmployees
{
    public class Command : IRequest<BulkImportResult>
    {
        public required Stream FileStream { get; set; }
    }

    public class Handler(AppDbContext context) : IRequestHandler<Command, BulkImportResult>
    {
        public async Task<BulkImportResult> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request.FileStream == null || request.FileStream.Length == 0)
                throw new InvalidOperationException("Uploaded CSV file is empty.");

            using var reader = new StreamReader(request.FileStream);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                PrepareHeaderForMatch = args => args.Header.Trim().ToLower()
            };

            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<EmployeeCsvRecord>().ToList();

            var result = new BulkImportResult();
            if (records.Count == 0) return result;

            var existingEmails = await context.Employees
                .Select(e => e.Email.ToLower())
                .ToHashSetAsync(cancellationToken);

            var newEmployees = new List<Employee>();
            int rowCounter = 1;

            foreach (var record in records)
            {
                rowCounter++;
                var normalizedEmail = record.Email?.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(normalizedEmail) ||
                    string.IsNullOrWhiteSpace(record.Name) ||
                    string.IsNullOrWhiteSpace(record.PhoneNo) ||
                    string.IsNullOrWhiteSpace(record.Address) ||
                    string.IsNullOrWhiteSpace(record.City) ||
                    string.IsNullOrWhiteSpace(record.State) ||
                    string.IsNullOrWhiteSpace(record.Country) ||
                    string.IsNullOrWhiteSpace(record.Pincode))
                {
                    result.SkippedCount++;
                    result.Errors.Add(new BulkImportError { RowNumber = rowCounter, Identifier = record.Email ?? "N/A", Reason = "Missing required fields." });
                    continue;
                }

                if (existingEmails.Contains(normalizedEmail))
                {
                    result.SkippedCount++;
                    result.Errors.Add(new BulkImportError { RowNumber = rowCounter, Identifier = normalizedEmail, Reason = "Email address already exists." });
                    continue;
                }

                if (!normalizedEmail.Contains('@'))
                {
                    result.SkippedCount++;
                    result.Errors.Add(new BulkImportError { RowNumber = rowCounter, Identifier = normalizedEmail, Reason = "Invalid email format." });
                    continue;
                }

                if (!DateTime.TryParse(record.HireDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedHireDate))
                {
                    result.SkippedCount++;
                    result.Errors.Add(new BulkImportError { RowNumber = rowCounter, Identifier = normalizedEmail, Reason = $"Invalid date format: '{record.HireDate}'." });
                    continue;
                }

                if (parsedHireDate > DateTime.UtcNow)
                {
                    result.SkippedCount++;
                    result.Errors.Add(new BulkImportError { RowNumber = rowCounter, Identifier = normalizedEmail, Reason = "Hire date cannot be in the future." });
                    continue;
                }

                var status = string.IsNullOrWhiteSpace(record.Status) ? "active" : record.Status.Trim().ToLower();
                if (status is not ("active" or "inactive"))
                {
                    result.SkippedCount++;
                    result.Errors.Add(new BulkImportError { RowNumber = rowCounter, Identifier = normalizedEmail, Reason = $"Invalid status value: '{record.Status}'." });
                    continue;
                }

                var employee = new Employee
                {
                    Id = Guid.NewGuid(),
                    Name = record.Name.Trim(),
                    Email = normalizedEmail,
                    HireDate = parsedHireDate,
                    PhoneNo = record.PhoneNo.Trim(),
                    ProfilePicture = record.ProfilePicture?.Trim() ?? string.Empty,
                    Status = status,
                    Address = record.Address.Trim(),
                    City = record.City.Trim(),
                    State = record.State.Trim(),
                    Country = record.Country.Trim(),
                    Pincode = record.Pincode.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                newEmployees.Add(employee);
                existingEmails.Add(normalizedEmail);
            }

            if (newEmployees.Count > 0)
            {
                await context.Employees.AddRangeAsync(newEmployees, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }

            result.ImportedCount = newEmployees.Count;
            return result;
        }
    }
}

public class EmployeeCsvRecord
{
    public string Name { get; set; } = string.Empty;
    public string HireDate { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNo { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
}