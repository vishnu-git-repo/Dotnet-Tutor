using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.Models.Entities;
using App.Models.Dtos;
using App.Data;
using App.Common;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/equipments")]
public class EquipmentController : ControllerBase
{
    private readonly AppDBContext _context;

    public EquipmentController(AppDBContext context)
    {
        _context = context;
    }

    [Authorize( Roles = "Admin")]
    [HttpPost()]
    public async Task<IActionResult> Create(CreateEquipmentDto dto)
    {
        try
        {
            var exists = await _context.Equipments
                .AnyAsync(e => e.Name == dto.Name);

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
                Condition = dto.Condition,
                Status = dto.Status,
                Count = dto.Count
            };

            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<Equipment>
            {
                Status = true,
                Message = "Equipment created successfully",
                Data = equipment
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, ErrorResponse());
        }
    }

    // READ ALL
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var equipments = await _context.Equipments.ToListAsync();

            return Ok(new ApiResponse<List<Equipment>>
            {
                Status = true,
                Message = "Equipments fetched successfully",
                Data = equipments
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, ErrorResponse());
        }
    }

    // READ BY ID
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var equipment = await _context.Equipments.FindAsync(id);

            if (equipment == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Status = false,
                    Message = "Equipment not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<Equipment>
            {
                Status = true,
                Message = "Equipment fetched successfully",
                Data = equipment
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, ErrorResponse());
        }
    }

    // UPDATE
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateEquipmentDto dto)
    {
        try
        {
            var equipment = await _context.Equipments.FindAsync(id);

            if (equipment == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Status = false,
                    Message = "Equipment not found",
                    Data = null
                });
            }

            equipment.Name = dto.Name;
            equipment.Description = dto.Description;
            equipment.Price = dto.Price;
            equipment.Category = dto.Category;
            equipment.Condition = dto.Condition;
            equipment.Status = dto.Status;
            equipment.Count = dto.Count;
            equipment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<Equipment>
            {
                Status = true,
                Message = "Equipment updated successfully",
                Data = equipment
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, ErrorResponse());
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var equipment   = await _context.Equipments.FindAsync(id);

            if (equipment == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Status = false,
                    Message = "Equipment not found",
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
        catch (Exception ex)
        {
            Console.WriteLine(ex);
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
