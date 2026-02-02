using FluentValidation;
using WorkShift.Api.Contracts.Employees;

namespace WorkShift.Api.Validation;

public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Role).InclusiveBetween(1, 2);
        RuleFor(x => x.DepartmentId).NotEmpty();
    }
}
