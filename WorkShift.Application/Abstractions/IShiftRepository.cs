using WorkShift.Domain.Entities;

namespace WorkShift.Application.Abstractions;

public interface IShiftRepository
{
    Task<List<Shift>> GetByEmployeeBetweenAsync(Guid employeeId, DateTime fromDate, DateTime toDate, CancellationToken ct = default);
    Task<List<Shift>> GetByDepartmentBetweenAsync(Guid departmentId, DateTime fromDate, DateTime toDate, CancellationToken ct = default);
    Task<Shift> AddAsync(Shift shift, CancellationToken ct = default);
    Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct = default);

}
