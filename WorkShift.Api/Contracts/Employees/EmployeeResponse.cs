namespace WorkShift.Api.Contracts.Employees;

public record EmployeeResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    int Role,
    Guid DepartmentId,
    bool IsActive
);
