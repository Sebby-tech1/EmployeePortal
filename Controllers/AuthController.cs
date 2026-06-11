using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using EmployeePortal.API.Data;
using EmployeePortal.API.DTOs;
using EmployeePortal.API.Services;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.RateLimiting;

namespace EmployeePortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("fixed")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly JwtService _jwtService;

        private bool IsValidInput(string input, string pattern)
        {
            return Regex.IsMatch(input, pattern);
        }

        private bool ContainsSqlInjection(string input)
{
    if (string.IsNullOrEmpty(input)) return false;

    string[] sqlKeywords = {
        "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "UNION", "EXEC", "EXECUTE",
        "CONVERT", "CHAR", "VARCHAR", "ALTER", "CREATE"
    };

    string upperInput = input.ToUpper();
    return sqlKeywords.Any(keyword => upperInput.Contains(keyword));
}

        private string SanitizeInput(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return input
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"","&quot;")
            .Replace("'", "&#x27;")
            .Replace("/", "&#X2F;");
        }

        public AuthController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginDto model)
{
    if (ContainsSqlInjection(model.Username) || ContainsSqlInjection(model.Password))
        return BadRequest("Invalid input detected.");

    model.Username = SanitizeInput(model.Username);

    

    if (!IsValidInput(model.Username, @"^[a-zA-Z0-9_]{3,20}$"))
        return BadRequest("Invalid username format.");

    if (!IsValidInput(model.Password, @"^(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*]).{8,}$"))
        return BadRequest("Invalid password format.");

    var employee = _context.Employees.FirstOrDefault(x => x.Username == model.Username);

    if (employee == null)
        return Unauthorized();

    bool valid = BCrypt.Net.BCrypt.Verify(model.Password, employee.PasswordHash);

    if (!valid)
        return Unauthorized();

    var token = _jwtService.GenerateToken(employee);

    return Ok(new
    {
        token,
                employee.Username,
                employee.Role
            });
            }
        }
    }


