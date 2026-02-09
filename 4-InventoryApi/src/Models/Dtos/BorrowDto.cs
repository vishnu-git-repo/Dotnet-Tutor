using App.Models.Entities;

namespace App.Models.Dtos;

public class CreateBorrowDto
{
    public int UserId { get; set; }
    public int EquipmentId { get; set; }
    public int EquipmentCount { get; set; }
     
    public int BorrowedDays { get; set; }

    public PaymentMode PaymentMode { get; set; } = PaymentMode.NotPaid;
    public decimal PaidAmount { get; set; } = 0;

    public DateTime AssignedDate { get; set; }
    public DateTime ReleaseDate { get; set; }

    public string? PreRemarks { get; set; }
}
