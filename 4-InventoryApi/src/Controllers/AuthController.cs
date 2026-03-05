using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            }
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

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
            return Unauthorized(new ApiResponse<object>
            {
                Status = false,
                Message = "User not found"
            });

        var oldOtps = _context.OTPHashes
            .Where(o => o.UserId == user.Id && !o.IsUsed);

        _context.OTPHashes.RemoveRange(oldOtps);

        var otp = new Random().Next(100000, 999999).ToString();

        var newOtp = new OTPHash
        {
            UserId = user.Id,
            OtpHashValue = BCrypt.Net.BCrypt.HashPassword(otp),
            Category = OTPCategory.Password,
            ExpireAt = DateTime.UtcNow.AddMinutes(5)
        };

        _context.OTPHashes.Add(newOtp);
        await _context.SaveChangesAsync();

        await _emailService.SendEmailAsync(
            user.Email, "Inventory password Reset", EmailTemplates.PasswordResetOtpEmail(user.Name, otp)
        );

        return Ok(new ApiResponse<object>
        {
            Status = true,
            Message = "OTP Sent"
        });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
            return Unauthorized(new ApiResponse<object>
            {
                Status = false,
                Message = "User not found"
            });

        var otpRecord = await _context.OTPHashes
            .Where(o => o.UserId == user.Id && !o.IsUsed)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (otpRecord == null)
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "OTP not found"
            });

        if (otpRecord.ExpireAt < DateTime.UtcNow)
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "OTP expired"
            });

        var isValid = BCrypt.Net.BCrypt.Verify(dto.Otp, otpRecord.OtpHashValue);

        if (!isValid)
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Invalid OTP"
            });

        otpRecord.IsUsed = true;
        await _context.SaveChangesAsync();

        var resetToken = _jwt.GeneratePasswordResetToken(user.Id, user.Email);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // true in production
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMinutes(10)
        };

        Response.Cookies.Append("reset_token", resetToken, cookieOptions);

        return Ok(new ApiResponse<object>
        {
            Status = false,
            Message = "OTP Verified. You can now reset password."
        });
    }

    [Authorize]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var tokenType = User.FindFirst("TokenType")?.Value;

        if (tokenType != "PasswordReset")
            return Unauthorized(new ApiResponse<object>
            {
                Status = false,
                Message = "User not found"
            });

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized(new ApiResponse<object>
            {
                Status = false,
                Message = "Invalid token - Token is missing"
            });

        var userId = int.Parse(userIdClaim.Value);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound(new ApiResponse<object>
            {
                Status = false,
                Message = "User not found"
            });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

        await _context.SaveChangesAsync();

        Response.Cookies.Delete("reset_token");

        return Ok(new ApiResponse<object>
        {
            Status = false,
            Message = "Password Reset Successful"
        });
    }
}
