namespace WorkShift.Api.Contracts.Employees;

public record UpdateEmployeeRequest(
    string FirstName,
    string LastName,
    int Role,
    Guid DepartmentId
);
