using WorkShift.Domain.Common;

namespace WorkShift.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } = null!;
}
