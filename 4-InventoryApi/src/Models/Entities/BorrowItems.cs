using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace App.Models.Entities;

public class BorrowItems
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BorrowId { get; set; }

    [Required]
    public int EquipmentId { get; set; }

    [Required]
    public int EquipmentItemId { get; set; }

    [Precision(18, 2)]
    public decimal EquipmentPrice { get; set; }

    public bool IsReturned { get; set; } = false;
    public DateTime? ReturnedAt { get; set; }

    [ForeignKey(nameof(BorrowId))]
    public Borrow? Borrow { get; set; }

    [ForeignKey(nameof(EquipmentId))]
    public Equipment? Equipment { get; set; }

    [ForeignKey(nameof(EquipmentItemId))]
    public EquipmentItem? EquipmentItem { get; set; }
}

