using Microsoft.EntityFrameworkCore;
using WorkShift.Application.Abstractions;
using WorkShift.Domain.Entities;
using WorkShift.Infrastructure.Data;

namespace WorkShift.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _db;

    public EmployeeRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Employee>> GetAllAsync(CancellationToken ct = default)
        => _db.Employees
              .Where(x => !x.IsDeleted)
              .OrderBy(x => x.LastName)
              .ThenBy(x => x.FirstName)
              .ToListAsync(ct);

    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Employees.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

    public Task<Employee?> GetByEmailAsync(string email, CancellationToken ct = default)
        => _db.Employees.FirstOrDefaultAsync(x => x.Email == email && !x.IsDeleted, ct);

    public async Task<Employee> AddAsync(Employee employee, CancellationToken ct = default)
    {
        _db.Employees.Add(employee);
        await _db.SaveChangesAsync(ct);
        return employee;
    }
}
