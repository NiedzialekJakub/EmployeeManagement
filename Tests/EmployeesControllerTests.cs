using Api.Controllers;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests;

public class EmployeesControllerTests
{
    [Fact]
    public async Task GetEmployee_ExistingId_ShouldReturnEmployee()
    {
        var employeeId = Guid.NewGuid();
        var expectedEmployee = new Employee
        {
            Id = employeeId,
            Name = "Jan Kowalski",
            Email = "jan.kowalski@example.com",
            HireDate = DateTime.UtcNow.AddYears(-1),
            PhoneNo = "+48123456789",
            Status = "active",
            Address = "Testowa 1",
            City = "Warsaw",
            State = "Mazowieckie",
            Country = "Poland",
            Pincode = "00001"
        };

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<IRequest<Employee>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEmployee);

        var controller = new EmployeesController();

        var httpContext = new DefaultHttpContext();
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(s => s.GetService(typeof(IMediator)))
            .Returns(mediatorMock.Object);

        httpContext.RequestServices = serviceProviderMock.Object;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var actionResult = await controller.GetEmployee(employeeId);

        Employee returnedEmployee;

        if (actionResult.Result is OkObjectResult okResult)
        {
            returnedEmployee = Assert.IsType<Employee>(okResult.Value);
        }
        else
        {
            returnedEmployee = Assert.IsType<Employee>(actionResult.Value);
        }

        Assert.NotNull(returnedEmployee);
        Assert.Equal(employeeId, returnedEmployee.Id);
        Assert.Equal("Jan Kowalski", returnedEmployee.Name);
    }
}