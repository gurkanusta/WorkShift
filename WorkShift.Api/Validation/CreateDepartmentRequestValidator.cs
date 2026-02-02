using FluentValidation;
using WorkShift.Api.Contracts.Departments;

namespace WorkShift.Api.Validation;

public class CreateDepartmentRequestValidator : AbstractValidator<CreateDepartmentRequest>
{
    public CreateDepartmentRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(100);
    }
}
