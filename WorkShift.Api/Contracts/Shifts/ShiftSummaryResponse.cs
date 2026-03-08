namespace WorkShift.Api.Contracts.Shifts;

public record ShiftSummaryResponse(
    Guid DepartmentId,
    string DepartmentName,
    DateTime WeekStart,
    DateTime WeekEnd,
    int TotalShifts,
    int DayShifts,
    int NightShifts,
    double TotalHours,
    List<EmployeeShiftCount> EmployeeBreakdown
);

public record EmployeeShiftCount(
    Guid EmployeeId,
    string FullName,
    int DayShifts,
    int NightShifts,
    double TotalHours
);
