using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models.Entities;

public enum EquipmentCategory
{
    Tools = 1,
    Electronics = 2,
    Vehicles = 3,
    Furniture = 4,
    SafetyGear = 5,
    Other = 6
}

public class Equipment
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int Count { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    public EquipmentCategory Category { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<EquipmentItem> EquipmentItems { get; set; } = new List<EquipmentItem>();
}
