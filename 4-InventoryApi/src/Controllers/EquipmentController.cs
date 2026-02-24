using App.Data;
using App.Models.Entities;
using App.Models.Dtos;
using App.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/equipments")]
public class EquipmentController : ControllerBase
{
    private readonly AppDBContext _context;

    public EquipmentController(AppDBContext context)
    {
        _context = context;
    }

    // CREATE EQUIPMENT (WITH ITEMS)
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateEquipment(CreateEquipmentDto dto)
    {
        try
        {
            if (dto.Count <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Status = false,
                    Message = "Count must be greater than zero",
                    Data = null
                });
            }

            var exists = await _context.Equipments
                .AnyAsync(e => EF.Functions.ILike(e.Name, dto.Name));

            if (exists)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Status = false,
                    Message = "Equipment name already exists",
                    Data = null
                });
            }

            var equipment = new Equipment
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Category = dto.Category,
                Count = dto.Count,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                EquipmentItems = new List<EquipmentItem>()
            };

            for (int i = 0; i < dto.Count; i++)
            {
                equipment.EquipmentItems.Add(new EquipmentItem
                {
                    Condition = EquipmentCondition.New,
                    Status = EquipmentStatus.Available,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Status = true,
                Message = "Equipment created successfully",
                Data = new { equipment.Id }
            });
        }
        catch (Exception)
        {
            return StatusCode(500, ErrorResponse());
        }
    }

    
    // FILTERED LIST (GROUP + ITEM MODE)
    [Authorize]
    [HttpPost("filteredequipment")]
    public async Task<IActionResult> GetFilteredEquipments(GetFilteredEquipmentDto dto)
    {
        try
        {
            if (dto.IsGroup)
            {
                var baseQuery = _context.Equipments.AsQueryable();
                var totalCount = await baseQuery.CountAsync();
                if (!string.IsNullOrWhiteSpace(dto.SearchString))
                {
                    var search = dto.SearchString.ToLower();
                    baseQuery = baseQuery.Where(e => e.Name.ToLower().Contains(search));
                }

                // STEP 1: Get grouped result first (no enum conversion here)
                var groupedData = await baseQuery
                    .GroupBy(e => e.Category)
                    .Select(g => new
                    {
                        Category = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync(); 

                // STEP 2: Convert enum to string in memory
                var categoryCounts = groupedData
                    .ToDictionary(
                        x => x.Category.ToString(),
                        x => x.Count
                    );

                var query = baseQuery
                    .Include(e => e.EquipmentItems)
                    .AsQueryable();

                if (dto.Category != 0)
                    query = query.Where(e => e.Category == (EquipmentCategory)dto.Category);

                var data = await query
                    .OrderByDescending(e => e.CreatedAt)
                    .Skip((dto.PageNo - 1) * dto.RowCount)
                    .Take(dto.RowCount)
                    .Select(e => new
                    {
                        e.Id,
                        e.Name,
                        e.Description,
                        e.Price,
                        e.Category, 

                        TotalItems = e.EquipmentItems.Count(),
                        AvailableCount = e.EquipmentItems.Count(i => i.Status == EquipmentStatus.Available),
                        InUseCount = e.EquipmentItems.Count(i => i.Status == EquipmentStatus.InUse),
                        ReservedCount = e.EquipmentItems.Count(i => i.Status == EquipmentStatus.Reserved),
                        MaintenanceCount = e.EquipmentItems.Count(i => i.Status == EquipmentStatus.UnderMaintenance)
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Status = true,
                    Message = "Grouped equipments fetched",
                    Data = new
                    {
                        Items = data,
                        TotalCount = totalCount,
                        CategoryCounts = categoryCounts
                    }
                });
            }

            else
            {
                var query = _context.EquipmentItems
                    .Include(e => e.Equipment)
                    .AsQueryable();
                int totalCount;
                if (dto.EquipmentId != 0)
                {
                    query = query.Where(e => e.EquipmentId == dto.EquipmentId);
                    totalCount = await query.CountAsync();
                } else totalCount = await query.CountAsync();

                if (dto.Status != 0)
                    query = query.Where(e => e.Status == (EquipmentStatus)dto.Status);

                if (dto.Condition != 0)
                    query = query.Where(e => e.Condition == (EquipmentCondition)dto.Condition);

                if (dto.Category != 0)
                    query = query.Where(e => e.Equipment.Category == (EquipmentCategory)dto.Category);

                if (!string.IsNullOrWhiteSpace(dto.SearchString))
                {
                    var search = dto.SearchString.ToLower();
                    query = query.Where(e =>
                        e.Equipment.Name.ToLower().Contains(search));
                }

                var statusCounts = await query
                    .GroupBy(e => e.Status)
                    .Select(g => new { g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Key.ToString(), x => x.Count);

                var conditionCounts = await query
                    .GroupBy(e => e.Condition)
                    .Select(g => new { g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Key.ToString(), x => x.Count);

                var data = await query
                    .OrderByDescending(e => e.CreatedAt)
                    .ThenByDescending(e => e.Id)
                    .Skip((dto.PageNo - 1) * dto.RowCount)
                    .Take(dto.RowCount)
                    .Select(e => new
                    {
                        e.Id,
                        e.EquipmentId,
                        EquipmentName = e.Equipment.Name,
                        EquipmentCategory = e.Equipment.Category,
                        e.Equipment.Price,
                        e.Equipment.Description,
                        Status = (int)e.Status,
                        Condition = (int)e.Condition,
                        e.CreatedAt,
                        e.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Status = true,
                    Message = "Equipment items fetched",
                    Data = new
                    {
                        Items = data,
                        TotalCount = totalCount,
                        StatusCounts = statusCounts,
                        ConditionCounts = conditionCounts
                    }
                });
            }
        }
        catch (Exception)
        {
            return StatusCode(500, ErrorResponse());
        }
    }


    // UPDATE EQUIPMENT
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEquipment(int id, UpdateEquipmentDto dto)
    {
        try
        {
            var equipment = await _context.Equipments.FindAsync(id);

            if (equipment == null)
                return NotFound(new ApiResponse<object>
                {
                    Status = false,
                    Message = "Equipment not found",
                    Data = null
                });

            equipment.Name = dto.Name;
            equipment.Description = dto.Description;
            equipment.Price = dto.Price;
            equipment.Category = dto.Category;
            equipment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Status = true,
                Message = "Equipment updated successfully",
                Data = null
            });
        }
        catch (Exception)
        {
            return StatusCode(500, ErrorResponse());
        }
    }

    
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEquipment(int id)
    {
        try
        {
            var equipment = await _context.Equipments
                .Include(e => e.EquipmentItems)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipment == null)
                return NotFound(new ApiResponse<object>
                {
                    Status = false,
                    Message = "Equipment not found",
                    Data = null
                });

            if (equipment.EquipmentItems.Any())
            {
                return BadRequest(new ApiResponse<object>
                {
                    Status = false,
                    Message = "Cannot delete equipment with existing items",
                    Data = null
                });
            }

            _context.Equipments.Remove(equipment);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Status = true,
                Message = "Equipment deleted successfully",
                Data = null
            });
        }
        catch (Exception)
        {
            return StatusCode(500, ErrorResponse());
        }
    }


    private ApiResponse<object> ErrorResponse()
    {
        return new ApiResponse<object>
        {
            Status = false,
            Message = "Internal server error",
            Data = null
        };
    }
}
