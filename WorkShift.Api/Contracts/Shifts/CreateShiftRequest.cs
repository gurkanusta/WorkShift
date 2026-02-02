namespace WorkShift.Api.Contracts.Shifts;

public record CreateShiftRequest(
    Guid EmployeeId,
    Guid DepartmentId,
    DateTime Date,   
    int ShiftType
    
);
