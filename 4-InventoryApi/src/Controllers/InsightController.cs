using App.Data;
using App.Models.Entities;
using App.Models.Dtos;
using App.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Razor.TagHelpers;

[ApiController]
[Route("api/insight")]
public class InsightController : ControllerBase
{
    private readonly AppDBContext _context;
    public InsightController(AppDBContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin")]
    [Route("admin")]
    [HttpGet]
    public async Task<IActionResult> GetAdminInsightCounts()
    {
        try
        {

            // User
            var userQuery = _context.Users
            .Where( u => u.Role == UserRole.Client)
            .AsQueryable();
            var userTotalCount = await userQuery.CountAsync();
            var groupedUserStatusData = await userQuery
                .GroupBy(u => u.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Equipment
            var equipmentQuery = _context.EquipmentItems.AsQueryable();
            var equipmentTotalCount = await equipmentQuery.CountAsync();
            var groupedEquipmentStatusData = await equipmentQuery
                .GroupBy(e => e.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
            var groupedEquipmentConditionData = await equipmentQuery
                .GroupBy(e => e.Condition)
                .Select(g => new
                {
                    Condition = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Borrow 
            var borrowQuery = _context.Borrows.AsQueryable();
            var BorrowTotalCount = await borrowQuery.CountAsync();
            var groupedBorrowStatusData = await borrowQuery
                .GroupBy(b => b.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Structured Counts

            var userCounts = groupedUserStatusData
                .ToDictionary(
                    x => x.Status.ToString(),
                    x => x.Count
                );

            var equipmentStatusCounts = groupedEquipmentStatusData
                .ToDictionary(
                    x => x.Status.ToString(),
                    x => x.Count
                );
            var equipmentConditionCounts = groupedEquipmentConditionData
                .ToDictionary(
                    x => x.Condition.ToString(),
                    x => x.Count
                );
            var borrowCounts = groupedBorrowStatusData
                .ToDictionary(
                    x => x.Status.ToString(),
                    x => x.Count
                );


            return Ok(new ApiResponse<Object>
            {
                Status = true,
                Message = "Insight Counts Fetched successfully",
                Data =
                new {
                    User = new {
                        Total = userTotalCount,
                        Status = userCounts
                    },
                    Equipment = new
                    {
                        Total = equipmentTotalCount,
                        Status = equipmentStatusCounts,
                        Condition = equipmentConditionCounts
                    },
                    Borrow = new
                    {
                        Total = BorrowTotalCount,
                        Status = borrowCounts
                    }
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<Object>
            {
                Status = false,
                Message = $"Internal Server Error - {ex.Message}"
            });
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
