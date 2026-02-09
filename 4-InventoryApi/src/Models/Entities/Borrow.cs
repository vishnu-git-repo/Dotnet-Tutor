using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace App.Models.Entities;

public enum BorrowStatus
{
    Requested = 1,
    Accepted = 2,
    Assigned = 3,
    Pending = 4,
    Paid = 5,
    Approved = 6,
    Waitlisted = 7,
    Ack = 8,
    Closed = 9
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

    [Required]
    public int UserId { get; set; }

    [Required]
    public int EquipmentId { get; set; }

    [Required]
    public int BorrowedDays { get; set; }

    public int EquipmentCount { get; set; } = 1;
    public int ReturnedCount { get; set; } = 0;

    [Precision(18, 2)]
    public decimal EquipmentPrice { get; set; }

    [Precision(18, 2)]
    public decimal TotalPrice { get; set; }

    [Precision(18, 2)]
    public decimal PaidAmount { get; set; }

    [Precision(18, 2)]
    public decimal DueAmount { get; set; }

    [Precision(18, 2)]
    public decimal LateFee { get; set; } = 0;

    public PaymentMode PaymentMode { get; set; } = PaymentMode.NotPaid;
    public bool IsPaymentCompleted { get; set; } = false;
    public string? PaymentId { get; set; }

    public BorrowStatus Status { get; set; } = BorrowStatus.Requested;

    public DateTime? RequestedDate { get; set; }
    public DateTime? AcceptedDate { get; set; }
    public DateTime? AssingnedDate { get; set; }
    public DateTime? PendingDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? WaitlistedDate { get; set; }
    public DateTime? AckDate { get; set; }
    public DateTime? ClosedDate { get; set; }

    public string? PreRemarks { get; set; }
    public string? PostRemarks { get; set; }
    public string? AckRemarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [ForeignKey(nameof(EquipmentId))]
    public Equipment? Equipment { get; set; }
}
