using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models.Entities;
public enum EquipmentCondition
{
    New = 1,
    Good = 2,
    Damaged = 3,
    Retired = 4
}

public enum EquipmentStatus
{
    Available = 1,
    InUse = 2,
    Reserved = 3,
    UnderMaintenance = 4
}

public class EquipmentItem
{
    [Key]
    public int Id { get; set; }

    public int EquipmentId { get; set; }

    [ForeignKey(nameof(EquipmentId))]
    public Equipment Equipment { get; set; } = null!; 

    public EquipmentCondition Condition { get; set; }

    public EquipmentStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

}
