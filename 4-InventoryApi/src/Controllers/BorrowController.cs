using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.Models.Entities;
using App.Models.Dtos;
using App.Data;

[ApiController]
[Route("api/borrows")]
public class BorrowController : ControllerBase
{
    private readonly AppDBContext _context;

    public BorrowController(AppDBContext context)
    {
        _context = context;
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> CreateBorrow(CreateBorrowDto dto)
    {
        try
        {
            var equipment = await _context.Equipments
                .FirstOrDefaultAsync(e => e.Id == dto.EquipmentId);

            if (equipment == null)
                return BadRequest(new { status = false, message = "Equipment not found", data = (object?)null });

            var totalPrice = dto.BorrowedDays * equipment.Price * dto.EquipmentCount;
            var dueAmount = totalPrice - dto.PaidAmount;

            var borrow = new Borrow
            {
                UserId = dto.UserId,
                EquipmentId = dto.EquipmentId,
                BorrowedDays = dto.BorrowedDays,
                EquipmentPrice = equipment.Price,
                EquipmentCount = dto.EquipmentCount,
                TotalPrice = totalPrice,
                PaidAmount = dto.PaidAmount,
                DueAmount = dueAmount,
                PaymentMode = dto.PaymentMode,
                AssignedDate = dto.AssignedDate,
                ReleaseDate = dto.ReleaseDate,
                PreRemarks = dto.PreRemarks,
                Status = BorrowStatus.Requested
            };

            _context.Borrows.Add(borrow);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = true,
                message = "Borrow created successfully",
                data = borrow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                status = false,
                message = "Something went wrong",
                error = ex.Message
            });
        }
    }

    // READ ALL
    [HttpGet]
    public async Task<IActionResult> GetAllBorrows()
    {
        var borrows = await _context.Borrows
            .Include(b => b.User)
            .Include(b => b.Equipment)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return Ok(new
        {
            status = true,
            message = "Borrow list",
            data = borrows
        });
    }

    // READ BY ID
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBorrowById(int id)
    {
        var borrow = await _context.Borrows
            .Include(b => b.User)
            .Include(b => b.Equipment)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (borrow == null)
            return NotFound(new { status = false, message = "Borrow not found", data = (object?)null });

        return Ok(new
        {
            status = true,
            message = "Borrow details",
            data = borrow
        });
    }

    // UPDATE STATUS / PAYMENT
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateBorrow(int id, CreateBorrowDto dto)
    {
        try
        {
            var borrow = await _context.Borrows.FindAsync(id);

            if (borrow == null)
                return NotFound(new { status = false, message = "Borrow not found", data = (object?)null });

            borrow.PaidAmount = dto.PaidAmount;
            borrow.DueAmount = borrow.TotalPrice - dto.PaidAmount;
            borrow.PaymentMode = dto.PaymentMode;
            borrow.UpdatedAt = DateTime.UtcNow;

            _context.Borrows.Update(borrow);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = true,
                message = "Borrow updated successfully",
                data = borrow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                status = false,
                message = "Something went wrong",
                error = ex.Message
            });
        }
    }

    // DELETE
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBorrow(int id)
    {
        var borrow = await _context.Borrows.FindAsync(id);

        if (borrow == null)
            return NotFound(new { status = false, message = "Borrow not found", data = (object?)null });

        _context.Borrows.Remove(borrow);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            status = true,
            message = "Borrow deleted successfully",
            data = (object?)null
        });
    }
}
