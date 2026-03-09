using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace App.Models.Entities;

public enum PaymentStatus
{
    Success = 1,
    Fails = 2
}

public enum PaymentMode
{
    Cash = 1,
    RazorPay = 2,
    NotPaid = 3
}

public class Payments
{
    [Key]
    public int Id {get; set;}
    public int BorrowId {get; set;}
    public int UserId {get; set;}

    [Precision(18,2)]
    public decimal Price {get; set;}

    public PaymentMode PaymentMode { get; set; } = PaymentMode.NotPaid;
    public PaymentStatus Status {get; set;}
    public string? RazorpayOrderId { get; set; }
    public string? RazorpayPaymentId { get; set; }
    public string? RazorpaySignature { get; set; }
    public DateTime? PaymentInitiatedDate { get; set; }
    public DateTime? PaymentCompletedDate { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [ForeignKey(nameof(BorrowId))]
    public Borrow? Borrow {get; set;}
}