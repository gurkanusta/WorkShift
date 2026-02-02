using Microsoft.AspNetCore.Mvc;
using WorkShift.Api.Contracts.Employees;
using WorkShift.Application.Abstractions;
using WorkShift.Domain.Entities;
using WorkShift.Domain.Enums;

namespace WorkShift.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _employees;
    private readonly IDepartmentRepository _departments;

    public EmployeesController(IEmployeeRepository employees, IDepartmentRepository departments)
    {
        _employees = employees;
        _departments = departments;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmployeeResponse>>> GetAll(CancellationToken ct)
    {
        var items = await _employees.GetAllAsync(ct);
        var result = items.Select(x => new EmployeeResponse(
            x.Id, x.FirstName, x.LastName, x.Email, (int)x.Role, x.DepartmentId, x.IsActive
        )).ToList();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeResponse>> Create(CreateEmployeeRequest req, CancellationToken ct)
    {
        var firstName = (req.FirstName ?? "").Trim();
        var lastName = (req.LastName ?? "").Trim();
        var email = (req.Email ?? "").Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            return BadRequest("FirstName and LastName are required.");

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is required.");

        if (!Enum.IsDefined(typeof(RoleType), req.Role))
            return BadRequest("Role is invalid. Use 1=Admin or 2=Staff.");

        var dep = await _departments.GetByIdAsync(req.DepartmentId, ct);
        if (dep is null)
            return BadRequest("DepartmentId is invalid.");

        var existing = await _employees.GetByEmailAsync(email, ct);
        if (existing is not null)
            return Conflict("Email already exists.");

        var employee = new Employee
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Role = (RoleType)req.Role,
            DepartmentId = req.DepartmentId,
            IsActive = true
        };

        var created = await _employees.AddAsync(employee, ct);

        return CreatedAtAction(nameof(GetAll), new { id = created.Id },
            new EmployeeResponse(created.Id, created.FirstName, created.LastName, created.Email,
                (int)created.Role, created.DepartmentId, created.IsActive));
    }
}
