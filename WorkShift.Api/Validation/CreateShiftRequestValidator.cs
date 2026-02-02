using FluentValidation;
using WorkShift.Api.Contracts.Shifts;

namespace WorkShift.Api.Validation;

public class CreateShiftRequestValidator : AbstractValidator<CreateShiftRequest>
{
    public CreateShiftRequestValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.DepartmentId).NotEmpty();
        RuleFor(x => x.ShiftType).InclusiveBetween(1, 2);
        RuleFor(x => x.Date).NotEmpty();
    }
}
