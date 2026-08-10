using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Api.Controllers;

public class EmployeesController(AppDbContext context) : BaseApiController
{
    // GET: api/employees - pobranie wszystkich pracowników
    [HttpGet("employees")]
    public async Task<ActionResult<List<Employee>>> GetEmployees()
    {
        return await context.Employees.ToListAsync();
    }

    // GET: api/employee/{id} - pobranie pojedynczego pracownika po ID
    [HttpGet("employee/{id:guid}")]
    public async Task<ActionResult<Employee>> GetEmployee(Guid id)
    {
        var employee = await context.Employees.FindAsync(id);

        if (employee == null) return NotFound();

        return employee;
    }
}