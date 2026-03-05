using App.Models.Entities;

namespace App.Models.Dtos;

public class CreateEquipmentDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; } = 0.00m;
    public EquipmentCategory Category { get; set; } = EquipmentCategory.Other;
    public int Count { get; set; } = 1;
} 

public class CreateEquipmentItemDto
{
    public required int EquipmentId {get; set;}
    public required int Count {get; set;}
}

public class GetFilteredEquipmentDto
{
    public required int RowCount {get; set;}
    public required int PageNo {get; set;}
    public string? SearchString {get; set;}
    public required bool IsGroup {get; set;}
    public required int Condition {get; set;} = 0;
    public required int Category {get; set;} = 0;
    public required int Status {get; set;} = 0;
    public int? EquipmentId {get; set;}
}

public class GetEquipmentGroupItemsDto
{
    public required int RowCount {get; set;}
    public required int PageNo {get; set;}
    public required int Condition {get; set;} = 0;
    public required int Status {get; set;} = 0;
    public int? EquipmentId {get; set;}
}


public class UpdateEquipmentDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; } = 0.00m;
    public EquipmentCategory Category { get; set; } = EquipmentCategory.Other;
}

public class UpdateEquipmentStatusDto
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public EquipmentStatus Status { get; set; }
}

public class UpdateEquipmentConditionDto
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public EquipmentCondition Condition { get; set; }
}


//Response
public class GetEquipmentResponseDto
{
    public int Id {get; set;}
    public int EquipmentId {get; set;}
    public string EquipmentName {get; set;} = "";
    public string Description {get; set;} = "";
    public decimal Price {get; set;}
    public int Condition {get; set;}
    public int Category {get; set;}
    public int Status {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
}

public class EquipmentGroupResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Category { get; set; }

    public int TotalItems { get; set; }
    public int AvailableCount { get; set; }
    public int InUseCount { get; set; }
    public int ReservedCount { get; set; }
    public int MaintenanceCount { get; set; }
}

public class EquipmentItemListDto
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public string EquipmentName { get; set; } = "";
    public string? Description { get; set; }

    public int Status { get; set; }
    public int Condition { get; set; }

    public DateTime CreatedAt { get; set; }
}

