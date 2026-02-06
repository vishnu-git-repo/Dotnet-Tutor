using App.Models.Entities;
namespace App.Models.Dtos;

public class CreateEquipmentDto
{
    public string Name {get; set;}
    public string? Description {get; set;}
    public decimal Price { get; set; } = 0.00;
    public EquipmentCategory Category { get; set; } = "Others";
    public EquipmentCondition Condition { get; set; } = "New";
    public EquipmentStatus Status { get; set; } = "Available";
    public int Count { get; set; } = 1;
}

