using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using Pmjay.Api.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly AgraDbContext _context;

    public AuthController(IConfiguration config, AgraDbContext context)
    {
        _config = config;
        _context = context;
    }

    // ===============================
    // LOGIN
    // ===============================
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserName == model.UserName);

        if (user == null)
            return Unauthorized("Invalid username or password");

        if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            return Unauthorized("Invalid username or password");

        var token = GenerateJwtToken(user);

        return Ok(new UserLogInModel
        {
            UserId = user.Id,
            FullName = user.FullName,
            RoleId = user.RoleId,
            RoleName = user.Role.RoleName,   // ✅ SAFE NOW
            IsAuthenticated = true,
            AccessToken = token
        });
    }

    // ===============================
    // JWT TOKEN
    // ===============================
    private string GenerateJwtToken(User user)
    {
        if (user.Role == null)
            throw new Exception("ROLE IS NULL");
        var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Name, user.FullName),
    // Use ClaimTypes.Role so Blazor's role checks find the claim
    new Claim(ClaimTypes.Role, user.Role.RoleName)
    };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"])
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(6),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}