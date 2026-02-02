namespace WorkShift.Api.Contracts.Shifts;

public record ShiftResponse(
    Guid Id,
    Guid EmployeeId,
    Guid DepartmentId,
    DateTime Date,
    int ShiftType,
    string StartTime,
    string EndTime
);
