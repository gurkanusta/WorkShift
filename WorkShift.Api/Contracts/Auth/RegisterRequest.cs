namespace WorkShift.Api.Contracts.Auth;

public record RegisterRequest(string Email, string Password, int Role, Guid? EmployeeId);
