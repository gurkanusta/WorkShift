using WorkShift.Domain.Common;
using WorkShift.Domain.Enums;

namespace WorkShift.Domain.Entities;

public class Employee : BaseEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;

    public Guid? UserId { get; set; }


    public RoleType Role { get; set; }

    public Guid DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
}
