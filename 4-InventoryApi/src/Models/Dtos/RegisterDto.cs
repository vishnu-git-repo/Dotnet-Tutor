using App.Models.Entities;

namespace App.Models.Dtos;

public class RegisterDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public UserRole Role { get; set; }

    public UserStatus Status { get; set; }

}