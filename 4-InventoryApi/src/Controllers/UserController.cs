using App.Common;
using App.Data;
using App.Models.Dtos;
using App.Models.Entities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly AppDBContext _context;

    public UserController(AppDBContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return Ok(new ApiResponse<object>
        {
            Status = true,
            Message = "User list",
            Data = users
        });
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound(new ApiResponse<object>
            {
                Status = false,
                Message = "User not found",
                Data = null
            });

        return Ok(new ApiResponse<object>
        {
            Status = true,
            Message = "User details",
            Data = user
        });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, UpdateUserDto dto)
    {
        var user = await _context.Users
            .FindAsync(int.Parse(id));

        if (user == null)
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "User not found",
                Data = null
            });

        user.Email = user.Email;
        user.PasswordHash = user.PasswordHash;
        user.Gender = dto.Gender;
        user.Phone = dto.Phone;
        user.Address = dto.Address;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            Status = true,
            Message = "User details updated",
            Data = user
        });
    }

    [Authorize]
    [HttpPut("password/{id}")]
    public async Task<IActionResult> UpdatePassword(string id, UpdateUserPasswordDto dto)
    {
        var user = await _context.Users
            .FindAsync(int.Parse(id));

        if (user == null)
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "User not found",
                Data = null
            });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            Status = true,
            Message = "Password updated successfully",
            Data = null
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("disable/{id:int}")]
    public async Task<IActionResult> DisableUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound(new ApiResponse<object>
            {
                Status = false,
                Message = "User not found",
                Data = null
            });
        user.Status = UserStatus.Inactive;
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            Status = true,
            Message = "User disabled successfully",
            Data = null
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("enable/{id:int}")]
    public async Task<IActionResult> EnableUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound(new ApiResponse<object>
            {
                Status = false,
                Message = "User not found",
                Data = null
            });
        user.Status = UserStatus.Active;
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            Status = true,
            Message = "User enabled successfully",
            Data = null
        });
    }
}
