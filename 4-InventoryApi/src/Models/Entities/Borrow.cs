using System.ComponentModel.DataAnnotations;

namespace App.Models.Entities;

// ================= ENUMS =================

public enum BorrowStatus
{
    Requested = 1,
    Approved = 2,
    Rejected = 3,
    Returned = 4
}

public enum PaymentMode
{
    Cash = 1,
    UPI = 2,
    Card = 3,
    BankTransfer = 4,
    NotPaid = 5
}

// ================= ENTITY =================

public class Borrow
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public int EquipmentId { get; set; }
    public int Count { get; set; } = 1;
    public PaymentMode PaymentMode { get; set; }
    public BorrowStatus Status { get; set; } = BorrowStatus.Requested;
    public int BorrowedDays { get; set; }
    public decimal Price { get; set; }
    public decimal Paid { get; set; }
    public decimal DueAmount { get; set; }
    public DateTime AssignedDate { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? PreRemarks { get; set; }
    public string? PostRemarks { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public Equipment? Equipment { get; set; }
}
