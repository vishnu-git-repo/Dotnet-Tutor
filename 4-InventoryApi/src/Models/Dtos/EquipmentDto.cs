using App.Models.Entities;

namespace App.Models.Dtos;

public class CreateEquipmentDto
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; } = 0.00m;


    public EquipmentCategory Category { get; set; } = EquipmentCategory.Other;

    public EquipmentCondition Condition { get; set; } = EquipmentCondition.New;

    public EquipmentStatus Status { get; set; } = EquipmentStatus.Available;

    public int Count { get; set; } = 1;
} 
