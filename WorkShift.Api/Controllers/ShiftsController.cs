using Microsoft.AspNetCore.Mvc;
using WorkShift.Api.Contracts.Shifts;
using WorkShift.Application.Abstractions;
using WorkShift.Domain.Entities;
using WorkShift.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace WorkShift.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShiftsController : ControllerBase
{
    // DEFAULT RULES
    private const double ShiftHours = 10.5;
    private const int MinRestHours = 12;
    private const double MaxWeeklyHours = 52.5; 

    private readonly IShiftRepository _shifts;
    private readonly IEmployeeRepository _employees;
    private readonly IDepartmentRepository _departments;

    public ShiftsController(IShiftRepository shifts, IEmployeeRepository employees, IDepartmentRepository departments)
    {
        _shifts = shifts;
        _employees = employees;
        _departments = departments;
    }


    [Authorize]
    [HttpGet("my-weekly")]
    public async Task<ActionResult<List<ShiftResponse>>> GetMyWeekly([FromQuery] DateTime date, CancellationToken ct)
    {
        var employeeIdClaim = User.FindFirst("employeeId")?.Value;
        if (string.IsNullOrWhiteSpace(employeeIdClaim) || !Guid.TryParse(employeeIdClaim, out var employeeId))
            return Forbid("No employeeId claim.");

        var (weekStart, weekEnd) = GetWeekRange(date.Date);
        var items = await _shifts.GetByEmployeeBetweenAsync(employeeId, weekStart, weekEnd, ct);

        var result = items
            .OrderBy(x => x.Date)
            .Select(x => new ShiftResponse(
                x.Id, x.EmployeeId, x.DepartmentId, x.Date.Date,
                (int)x.ShiftType,
                x.StartTime.ToString(@"hh\:mm"),
                x.EndTime.ToString(@"hh\:mm")
            ))
            .ToList();

        return Ok(result);
    }


    [HttpPost]
    public async Task<ActionResult<ShiftResponse>> Create(CreateShiftRequest req, CancellationToken ct)
    {
        if (req.EmployeeId == Guid.Empty) return BadRequest("EmployeeId is required.");
        if (req.DepartmentId == Guid.Empty) return BadRequest("DepartmentId is required.");
        if (!Enum.IsDefined(typeof(ShiftType), req.ShiftType)) return BadRequest("ShiftType is invalid. Use 1=Day or 2=Night.");

        var employee = await _employees.GetByIdAsync(req.EmployeeId, ct);
        if (employee is null) return BadRequest("EmployeeId is invalid.");
        if (!employee.IsActive) return BadRequest("Employee is not active.");

        var dep = await _departments.GetByIdAsync(req.DepartmentId, ct);
        if (dep is null) return BadRequest("DepartmentId is invalid.");

        if (employee.DepartmentId != req.DepartmentId)
            return BadRequest("Employee does not belong to this department.");

        var date = req.Date.Date;
        var shiftType = (ShiftType)req.ShiftType;

       
        var (start, end) = GetShiftDateTimeRange(date, shiftType);
        var startTime = start.TimeOfDay;
        var endTime = end.TimeOfDay;

        
        var nearby = await _shifts.GetByEmployeeBetweenAsync(req.EmployeeId, date.AddDays(-1), date.AddDays(1), ct);

       
        foreach (var s in nearby)
        {
            var (sStart, sEnd) = GetShiftDateTimeRange(s.Date.Date, s.ShiftType, s.StartTime, s.EndTime);
            if (Overlaps(start, end, sStart, sEnd))
                return Conflict("Shift conflicts with an existing shift for this employee.");
        }

        
        var previousShifts = await _shifts.GetByEmployeeBetweenAsync(req.EmployeeId, date.AddDays(-14), date, ct);
        var lastEnd = previousShifts
            .Select(s => GetShiftDateTimeRange(s.Date.Date, s.ShiftType, s.StartTime, s.EndTime).End)
            .Where(e => e <= start)
            .OrderByDescending(e => e)
            .FirstOrDefault();

        if (lastEnd != default)
        {
            var restHours = (start - lastEnd).TotalHours;
            if (restHours < MinRestHours)
                return Conflict($"Minimum rest rule violated. Required: {MinRestHours}h.");
        }

       
        var (weekStart, weekEnd) = GetWeekRange(date);
        var weekShifts = await _shifts.GetByEmployeeBetweenAsync(req.EmployeeId, weekStart, weekEnd, ct);

        var weekHours = weekShifts
            .Select(s => GetShiftDateTimeRange(s.Date.Date, s.ShiftType, s.StartTime, s.EndTime))
            .Sum(r => (r.End - r.Start).TotalHours);

        if (weekHours + ShiftHours > MaxWeeklyHours)
            return Conflict("Weekly hours limit exceeded.");

       
        var shift = new Shift
        {
            EmployeeId = req.EmployeeId,
            DepartmentId = req.DepartmentId,
            Date = date,
            ShiftType = shiftType,
            StartTime = startTime,
            EndTime = endTime
        };

        var created = await _shifts.AddAsync(shift, ct);

        return Created(string.Empty, new ShiftResponse(
    created.Id,
    created.EmployeeId,
    created.DepartmentId,
    created.Date.Date,
    (int)created.ShiftType,
    created.StartTime.ToString(@"hh\:mm"),
    created.EndTime.ToString(@"hh\:mm")
));

    }


    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await _shifts.SoftDeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }



    private static (DateTime Start, DateTime End) GetWeekRange(DateTime anyDate)
    {
        
        int diff = (7 + (anyDate.DayOfWeek - DayOfWeek.Monday)) % 7;
        var start = anyDate.AddDays(-diff).Date;
        var end = start.AddDays(6).Date;
        return (start, end);
    }

    private static (DateTime Start, DateTime End) GetShiftDateTimeRange(DateTime date, ShiftType type)
    {
        
        return type switch
        {
            ShiftType.Day => (date.AddHours(8), date.AddHours(18).AddMinutes(30)),
            ShiftType.Night => (date.AddHours(20), date.AddDays(1).AddHours(6).AddMinutes(30)),
            _ => (date.AddHours(8), date.AddHours(18).AddMinutes(30))
        };
    }

    private static (DateTime Start, DateTime End) GetShiftDateTimeRange(DateTime date, ShiftType type, TimeSpan startTime, TimeSpan endTime)
    {
        
        var start = date.Date.Add(startTime);
        var end = date.Date.Add(endTime);
        if (endTime <= startTime) end = end.AddDays(1);
        return (start, end);
    }

    private static bool Overlaps(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd)
        => aStart < bEnd && bStart < aEnd;
}
