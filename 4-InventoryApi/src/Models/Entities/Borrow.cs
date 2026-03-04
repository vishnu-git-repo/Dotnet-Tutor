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

public class Borrow
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }


    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime ExpectedReturnDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }


    [Precision(18, 2)]
    public decimal TotalPrice { get; set; }
    [Precision(18, 2)]
    public decimal PaidAmount { get; set; }
    [Precision(18, 2)]
    public decimal DueAmount { get; set; }
    [Precision(18, 2)]
    public decimal LateFee { get; set; } = 0;
    public bool IsPaymentCompleted { get; set; } = false;

    public BorrowStatus Status { get; set; } = BorrowStatus.Requested;
    public int EquipmentCounts { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    public ICollection<BorrowItems>? BorrowItems { get; set; }
}