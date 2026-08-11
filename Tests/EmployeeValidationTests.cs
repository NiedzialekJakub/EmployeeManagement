using Application.Employees.Commands;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Xunit;

namespace Tests;

public class EmployeeValidationTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Validate_FutureHireDate_ShouldFailValidation()
    {
        using var context = GetInMemoryDbContext();
        var validator = new CreateEmployeeValidator(context);

        var command = new CreateEmployee.Command
        {
            Employee = new Domain.Employee
            {
                Name = "Jan Kowalski",
                Email = "jan.kowalski@example.com",
                HireDate = DateTime.UtcNow.AddDays(5), // future date
                PhoneNo = "+48123456789",
                Status = "active",
                Address = "Testowa 1",
                City = "Warsaw",
                State = "Mazowieckie",
                Country = "Poland",
                Pincode = "00001"
            }
        };

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName.Contains("HireDate"));
    }

    [Fact]
    public async Task Validate_InvalidStatus_ShouldFailValidation()
    {
        using var context = GetInMemoryDbContext();
        var validator = new CreateEmployeeValidator(context);

        var command = new CreateEmployee.Command
        {
            Employee = new Domain.Employee
            {
                Name = "Jan Kowalski",
                Email = "jan.kowalski@example.com",
                HireDate = DateTime.UtcNow.AddYears(-1),
                PhoneNo = "+48123456789",
                Status = "invalid_status", // invalid status
                Address = "Testowa 1",
                City = "Warsaw",
                State = "Mazowieckie",
                Country = "Poland",
                Pincode = "00001"
            }
        };

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName.Contains("Status"));
    }

    [Fact]
    public async Task Validate_DuplicateEmail_ShouldFailValidation()
    {
        using var context = GetInMemoryDbContext();
        context.Employees.Add(new Domain.Employee
        {
            Id = Guid.NewGuid(),
            Name = "Existing Employee",
            Email = "jan.kowalski@example.com",
            HireDate = DateTime.UtcNow.AddYears(-1),
            PhoneNo = "+48123456789",
            Status = "active",
            Address = "Testowa 1",
            City = "Warsaw",
            State = "Mazowieckie",
            Country = "Poland",
            Pincode = "00001"
        });
        await context.SaveChangesAsync();

        var validator = new CreateEmployeeValidator(context);

        var command = new CreateEmployee.Command
        {
            Employee = new Domain.Employee
            {
                Name = "Jan Kowalski",
                Email = "jan.kowalski@example.com", // duplicate email
                HireDate = DateTime.UtcNow.AddYears(-1),
                PhoneNo = "+48123456789",
                Status = "active",
                Address = "Testowa 1",
                City = "Warsaw",
                State = "Mazowieckie",
                Country = "Poland",
                Pincode = "00001"
            }
        };

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName.Contains("Email"));
    }
}