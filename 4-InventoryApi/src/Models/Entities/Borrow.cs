using System.ComponentModel.DataAnnotations;

namespace App.Models.Entities;

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

public class Borrow
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }
    public int EquipmentId { get; set; }

    public int BorrowedDays { get; set; }

    public int EquipmentCount { get; set; } = 1;
    public int ReturnedCount { get; set; } = 0;

    public decimal EquipmentPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public decimal LateFee { get; set; } = 0;

    public PaymentMode PaymentMode { get; set; } = PaymentMode.NotPaid;
    public bool IsPaymentCompleted { get; set; } = false;
    public DateTime? PaymentDate { get; set; }

    public BorrowStatus Status { get; set; } = BorrowStatus.Requested;

    public DateTime AssignedDate { get; set; }
    public DateTime ReleaseDate { get; set; }

    public string? PreRemarks { get; set; }
    public string? PostRemarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public Equipment? Equipment { get; set; }
}
