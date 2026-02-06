using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models.Entities;
public enum EquipmentCondition
{
    New = 1,
    Good = 2,
    NeedsRepair = 3,
    Damaged = 4,
    Retired = 5
}

public enum EquipmentCategory
{
    Tools = 1,
    Electronics = 2,
    Vehicles = 3,
    Furniture = 4,
    SafetyGear = 5,
    Other = 6
}

public enum EquipmentStatus
{
    Available = 1,
    InUse = 2,
    Reserved = 3,
    UnderMaintenance = 4
}

public class Equipment
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    public EquipmentCategory Category { get; set; }

    public EquipmentCondition Condition { get; set; }

    public EquipmentStatus Status { get; set; }

    public int Count { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
