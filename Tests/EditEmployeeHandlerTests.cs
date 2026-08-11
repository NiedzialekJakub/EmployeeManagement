using Application.Employees.Commands;
using AutoMapper;
using Domain;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence;
using Xunit;

namespace Tests;

public class EditEmployeeHandlerTests
{
    private static AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static IMapper GetMockedMapper()
    {
        var mapperMock = new Mock<IMapper>();

        mapperMock.Setup(m => m.Map(It.IsAny<Employee>(), It.IsAny<Employee>()))
            .Callback<object, object>((srcObj, destObj) =>
            {
                if (srcObj is Employee src && destObj is Employee dest)
                {
                    dest.Name = src.Name;
                    dest.Email = src.Email;
                    dest.PhoneNo = src.PhoneNo;
                    dest.Status = src.Status;
                    dest.Address = src.Address;
                    dest.City = src.City;
                    dest.State = src.State;
                    dest.Country = src.Country;
                    dest.Pincode = src.Pincode;
                }
            });

        return mapperMock.Object;
    }

    [Fact]
    public async Task Handle_ExistingEmployee_ShouldUpdateDataSuccessfully()
    {
        using var context = GetInMemoryDbContext();
        var mapper = GetMockedMapper();
        var employeeId = Guid.NewGuid();

        var existingEmployee = new Employee
        {
            Id = employeeId,
            Name = "Jan Kowalski",
            Email = "jan.kowalski@example.com",
            HireDate = DateTime.UtcNow.AddYears(-2),
            PhoneNo = "+48123456789",
            Status = "active",
            Address = "Stary Adres 1",
            City = "Warsaw",
            State = "Mazowieckie",
            Country = "Poland",
            Pincode = "00001",
            CreatedAt = DateTime.UtcNow
        };

        context.Employees.Add(existingEmployee);
        await context.SaveChangesAsync();

        var handler = new EditEmployee.Handler(context, mapper);

        var command = new EditEmployee.Command
        {
            Employee = new Employee
            {
                Id = employeeId,
                Name = "Jan Kowalski Updated",
                Email = "jan.kowalski@example.com",
                HireDate = existingEmployee.HireDate,
                PhoneNo = "+48987654321", // changed phone
                Status = "inactive",       // changed status
                Address = "Nowy Adres 10", // changed adress
                City = "Krakow",           // changed city
                State = "Malopolskie",
                Country = "Poland",
                Pincode = "30001"
            }
        };

        await handler.Handle(command, CancellationToken.None);

        var updatedInDb = await context.Employees.FindAsync(employeeId);
        Assert.NotNull(updatedInDb);
        Assert.Equal("Jan Kowalski Updated", updatedInDb.Name);
        Assert.Equal("inactive", updatedInDb.Status);
        Assert.Equal("Nowy Adres 10", updatedInDb.Address);
        Assert.Equal("Krakow", updatedInDb.City);
    }
}