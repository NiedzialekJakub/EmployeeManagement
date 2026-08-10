using Application.Employees.Commands;
using Application.Employees.Queries;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Api.Controllers;

public class EmployeesController() : BaseApiController
{
    // GET: api/employees - pobranie wszystkich pracowników
    [HttpGet("employees")]
    public async Task<ActionResult<List<Employee>>> GetEmployees()
    {
        return await Mediator.Send(new GetEmployeeList.Query());
    }

    // GET: api/employee/{id} - pobranie pojedynczego pracownika po ID
    [HttpGet("employee/{id:guid}")]
    public async Task<ActionResult<Employee>> GetEmployee(Guid id)
    {
        var employee = await Mediator.Send(new GetEmployee.Query { Id = id });

        if (employee == null) return NotFound();

        return employee;
    }

    [HttpPost("employee")]
    public async Task<ActionResult<Guid>> CreateEmployee(Employee employee)
    {
        var id = await Mediator.Send(new CreateEmployee.Command { Employee = employee });

        return CreatedAtAction(nameof(GetEmployee), new { id }, id);
    }

    [HttpPut("employee/{id:guid}")]
    public async Task<ActionResult> EditEmployee(Guid id, Employee employee)
    {
        employee.Id = id;

        var success = await Mediator.Send(new EditEmployee.Command { Employee = employee });

        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("employee/{id:guid}")]
    public async Task<ActionResult> DeleteEmployee(Guid id)
    {
        var success = await Mediator.Send(new DeleteEmployee.Command { Id = id });

        if (!success) return NotFound();

        return NoContent();
    }

    [HttpPost("employees/bulk")]
    public async Task<ActionResult> BulkImportEmployees(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Please upload a valid non-empty CSV file." });

        using var stream = file.OpenReadStream();
        var result = await Mediator.Send(new BulkImportEmployees.Command { FileStream = stream });

        return Ok(result);
    }
}