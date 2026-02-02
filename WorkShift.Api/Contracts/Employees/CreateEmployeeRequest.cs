namespace WorkShift.Api.Contracts.Employees;

public record CreateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    int Role,          
    Guid DepartmentId
);
