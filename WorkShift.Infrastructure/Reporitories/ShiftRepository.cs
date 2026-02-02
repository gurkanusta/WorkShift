using Microsoft.EntityFrameworkCore;
using WorkShift.Application.Abstractions;
using WorkShift.Domain.Entities;
using WorkShift.Infrastructure.Data;

namespace WorkShift.Infrastructure.Repositories;

public class ShiftRepository : IShiftRepository
{
    private readonly AppDbContext _db;

    public ShiftRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Shift>> GetByEmployeeBetweenAsync(Guid employeeId, DateTime fromDate, DateTime toDate, CancellationToken ct = default)
        => _db.Shifts
              .Where(x => !x.IsDeleted
                       && x.EmployeeId == employeeId
                       && x.Date >= fromDate.Date
                       && x.Date <= toDate.Date)
              .ToListAsync(ct);

    public Task<List<Shift>> GetByDepartmentBetweenAsync(Guid departmentId, DateTime fromDate, DateTime toDate, CancellationToken ct = default)
        => _db.Shifts
              .Where(x => !x.IsDeleted
                       && x.DepartmentId == departmentId
                       && x.Date >= fromDate.Date
                       && x.Date <= toDate.Date)
              .ToListAsync(ct);

    public async Task<Shift> AddAsync(Shift shift, CancellationToken ct = default)
    {
        _db.Shifts.Add(shift);
        await _db.SaveChangesAsync(ct);
        return shift;
    }

    public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var shift = await _db.Shifts.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (shift is null) return false;

        shift.IsDeleted = true;
        shift.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return true;
    }

}
