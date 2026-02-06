using System.ComponentModel.DataAnnotations;

namespace App.Models.Entities;

public enum UserRole
{
    Admin = 1,
    Client = 2
}

public enum UserStatus
{
    Active = 1,
    Inactive = 2
}


public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(150)]
    public string Email { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    public string? Gender { get; set; }

    public string? Address { get; set; }

    [MaxLength(15)]
    public string? Phone { get; set; }

    public UserRole Role { get; set; } = UserRole.Client;

    public UserStatus Status { get; set; } = UserStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();
}
