using Microsoft.EntityFrameworkCore;
using WorkShift.Application.Abstractions;
using WorkShift.Domain.Entities;
using WorkShift.Infrastructure.Data;

namespace WorkShift.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _db;

    public DepartmentRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Department>> GetAllAsync(CancellationToken ct = default)
        => _db.Departments
              .Where(x => !x.IsDeleted)
              .OrderBy(x => x.Name)
              .ToListAsync(ct);

    public Task<Department?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Departments.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

    public async Task<Department> AddAsync(Department department, CancellationToken ct = default)
    {
        _db.Departments.Add(department);
        await _db.SaveChangesAsync(ct);
        return department;
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
        => _db.Departments.AnyAsync(x => !x.IsDeleted && x.Name == name, ct);
}
