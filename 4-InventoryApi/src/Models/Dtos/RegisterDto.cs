using App.Models.Entities;

namespace App.Models.Dtos;

public class RegisterDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }

    public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
}
