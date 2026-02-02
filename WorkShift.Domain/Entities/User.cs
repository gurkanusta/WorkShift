using WorkShift.Domain.Common;
using WorkShift.Domain.Enums;

namespace WorkShift.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public RoleType Role { get; set; } = RoleType.Staff;

    
    public Guid? EmployeeId { get; set; }
    public bool IsActive { get; set; } = true;
}
