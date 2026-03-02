using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using App.Models.Entities;
using App.Models.Dtos;
using App.Data;
using App.Services;
using App.Common;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace App.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDBContext _context;
    private readonly JwtService _jwt;
    private readonly IEmailService _emailService;

    public AuthController(AppDBContext context, JwtService jwt, IEmailService emailService)
    {
        _context = context;
        _jwt = jwt;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists)
                return BadRequest(new ApiResponse<object>
                {
                    Status = false,
                    Message = "User Already Exists",
                    Data = null
                });

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Gender = dto.Gender,
                Address = dto.Address,
                Phone = dto.Phone,
                Role = UserRole.Client,
                Status = UserStatus.Active
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendEmailAsync(
                        user.Email,
                        "Welcome To Inventory",
                        EmailTemplates.WelcomeEmail(user.Name)
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Email failed: " + ex.Message);
                }
            });

            return Ok(new ApiResponse<object>()
            {
                Status = true,
                Message = "Registration Successful",
                Data = new
                {
                    UserRole = user.Role,
                    UserId = user.Id
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Something went wrong");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        Console.WriteLine("Initializing the Login process>>>>>>>>");
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
            return Unauthorized(new ApiResponse<object>
                {
                    Status = false,
                    Message = "User Not Exists",
                    Data = null
                });
        if (user.Status == UserStatus.Inactive)
            return Unauthorized(new ApiResponse<object>
                {
                    Status = false,
                    Message = "You was blocked, Please contact admin",
                    Data = null
                });

        // Generate JWT
        var token = _jwt.GenerateToken(user.Id, user.Email, user.Role);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddHours(24 * 1)
        };

        Response.Cookies.Append("jwt", token, cookieOptions);

        return Ok(new ApiResponse<Object>()
        {
            Status = true,
            Message = "Login Successful",
            Data = new
            {
                UserRole = user.Role,
                UserId = user.Id
            } // Token is stored in cookie, no need to send in body for security
        });
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok(new ApiResponse<Object>()
        {
            Status = true,
            Message = "Logout Success",
            Data = null
        });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> AuthMe()
    {
        Console.WriteLine("Checking the Auth>>>>>>>>>>>>>>>>");
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized(new ApiResponse<Object>()
                {
                    Status = false,
                    Message = "Token Missing",
                    Data = null
                });

            var userId = int.Parse(userIdClaim.Value);

            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Gender,
                    u.Address,
                    u.Phone,
                    u.Role,
                    u.Status
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("User not found");

            return Ok(new ApiResponse<object>()
            {
                Status = true,
                Message = "User fetched successfully",
                Data = user
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Something went wrong");
        }
    }
}
