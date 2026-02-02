using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorkShift.Api.Contracts.Auth;
using WorkShift.Api.Security;
using WorkShift.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using WorkShift.Domain.Entities;
using WorkShift.Domain.Enums;

namespace WorkShift.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _cfg;

    public AuthController(AppDbContext db, IConfiguration cfg)
    {
        _db = db;
        _cfg = cfg;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterRequest req, CancellationToken ct)
    {
        var email = (req.Email ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email)) return BadRequest("Email required.");
        if (string.IsNullOrWhiteSpace(req.Password) || req.Password.Length < 6) return BadRequest("Password min 6 chars.");
        if (!Enum.IsDefined(typeof(RoleType), req.Role)) return BadRequest("Role invalid. 1=Admin, 2=Staff.");

        var exists = await _db.Users.AnyAsync(x => x.Email == email && !x.IsDeleted, ct);
        if (exists) return Conflict("Email already exists.");

        
        if (req.EmployeeId.HasValue)
        {
            var emp = await _db.Employees.FirstOrDefaultAsync(x => x.Id == req.EmployeeId.Value && !x.IsDeleted, ct);
            if (emp is null) return BadRequest("EmployeeId invalid.");
        }

        var user = new User
        {
            Email = email,
            PasswordHash = PasswordHasher.Hash(req.Password),
            Role = (RoleType)req.Role,
            EmployeeId = req.EmployeeId,
            IsActive = true
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest req, CancellationToken ct)
    {
        var email = (req.Email ?? "").Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email && !x.IsDeleted, ct);
        if (user is null || !user.IsActive) return Unauthorized("Invalid credentials.");

        if (!PasswordHasher.Verify(req.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials.");

        var token = CreateToken(user);
        return Ok(new AuthResponse(token));
    }

    private string CreateToken(User user)
    {
        var jwt = _cfg.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
        };

        if (user.EmployeeId.HasValue)
            claims.Add(new Claim("employeeId", user.EmployeeId.Value.ToString()));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpiresMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
