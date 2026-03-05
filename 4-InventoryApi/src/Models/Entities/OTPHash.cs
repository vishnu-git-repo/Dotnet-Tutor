
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace App.Models.Entities;

public enum OTPCategory
{
    Password = 1
}

public class OTPHash
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    [Required]
    public string OtpHashValue { get; set; } = "";

    public OTPCategory Category { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ExpireAt { get; set; }

    public bool IsUsed { get; set; } = false;

    // Navigation
    public User User { get; set; } = null!;
}