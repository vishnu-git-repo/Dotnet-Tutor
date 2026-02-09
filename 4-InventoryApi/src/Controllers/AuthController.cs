using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

using App.Models.Entities;
using App.Models.Dtos;
using App.Data;
using App.Services; 



namespace App.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDBContext _context;
    private readonly JwtService _jwt;

    public AuthController(AppDBContext context, JwtService jwt)
    {
        _context = context;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists)
                return BadRequest("Already registered");

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

            return Ok("Registration successful");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);

            return StatusCode(500, "Something went wrong");
        }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        Console.WriteLine("Initializing the Login process>>>>>>>>");
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || user.Status == UserStatus.Inactive)
            return Unauthorized("User does not exist");

        var checkPass = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!checkPass)
            return Unauthorized("Invalid password");

        var token = _jwt.GenerateToken(user.Id, user.Email, user.Role);

        return Ok(new
        {
            token,
            user = new
            {
                user.Id,
                user.Name,
                user.Email,
                Role = user.Role.ToString()
            }
        });
    }


}
