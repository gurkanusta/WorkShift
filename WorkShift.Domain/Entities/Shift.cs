using WorkShift.Domain.Common;
using WorkShift.Domain.Enums;

namespace WorkShift.Domain.Entities;

public class Shift : BaseEntity
{
    public DateTime Date { get; set; }

    public ShiftType ShiftType { get; set; }

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public Guid EmployeeId { get; set; }
    public Guid DepartmentId { get; set; }
}
