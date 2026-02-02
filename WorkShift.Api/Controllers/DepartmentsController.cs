using Microsoft.AspNetCore.Mvc;
using WorkShift.Api.Contracts.Departments;
using WorkShift.Application.Abstractions;
using WorkShift.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace WorkShift.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentRepository _repo;

    public DepartmentsController(IDepartmentRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<List<DepartmentResponse>>> GetAll(CancellationToken ct)
    {
        var items = await _repo.GetAllAsync(ct);
        var result = items.Select(x => new DepartmentResponse(x.Id, x.Name)).ToList();
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<DepartmentResponse>> Create(CreateDepartmentRequest req, CancellationToken ct)
    {
        var name = (req.Name ?? "").Trim();

        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Department name is required.");

        var exists = await _repo.ExistsByNameAsync(name, ct);
        if (exists)
            return Conflict("Department already exists.");

        var department = new Department { Name = name };
        var created = await _repo.AddAsync(department, ct);

        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, new DepartmentResponse(created.Id, created.Name));
    }
}
