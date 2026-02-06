using App.Models.Entities;

namespace App.Models.Dtos;

public class CreateBorrowDto
{
    public int UserId { get; set; }
    public int EquipmentId { get; set; }
    public PaymentMode PaymentMode { get; set; } = "NotPaid";
    public BorrowStatus Status { get; set; } = "Requested";
    public int BorrowedDays { get; set; }
    public decimal Price { get; set; } 
    public decimal PaidAmount { get; set; } 
    public decimal DueAmount { get; set; }
    public DateTime AssignedDate { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? PreRemarks { get; set; }
    public string? PostRemarks { get; set; }
}


