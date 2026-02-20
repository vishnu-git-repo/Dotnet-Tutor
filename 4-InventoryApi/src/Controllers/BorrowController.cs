using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.Models.Entities;
using App.Models.Dtos;
using App.Data;
using Microsoft.AspNetCore.Authorization;
using App.Common;

[ApiController]
[Route("api/borrows")]
public class BorrowController : ControllerBase
{
    private readonly AppDBContext _context;

    public BorrowController(AppDBContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Client")]
    [HttpPost("request")]
    public async Task<IActionResult> RequestBorrow(RequestBorrowDto dto)
    {
        var equipment = await _context.Equipments.FindAsync(dto.EquipmentId);
        if (equipment == null)
            return BadRequest(new ApiResponse<Object>()
            {
                Status = false,
                Message = "Equipment not found",
                Data = null
            });

        // Check available items
        var equipmentItems = await _context.EquipmentItems
            .Where(e => e.EquipmentId == dto.EquipmentId)
            .Where(e => e.Status == EquipmentStatus.Available)
            .Take(dto.EquipmentCount)
            .ToListAsync();

        if (equipmentItems.Count < dto.EquipmentCount)
            return BadRequest(new ApiResponse<Object>()
            {
                Status = false,
                Message = "Not enough equipment available",
                Data = null
            });

        var totalPrice = dto.EquipmentCount * dto.EquipmentPrice;

        var createBorrow = new Borrow
        {
            UserId = dto.UserId,
            EquipmentId = dto.EquipmentId,
            EquipmentCount = dto.EquipmentCount,
            EquipmentPrice = dto.EquipmentPrice,
            TotalPrice = totalPrice,
            RequestedDate = DateTime.UtcNow
        };

        _context.Borrows.Add(createBorrow);
        await _context.SaveChangesAsync();

        foreach (var item in equipmentItems)
        {
            item.Status = EquipmentStatus.InUse;

            var borrowItem = new BorrowItems
            {
                BorrowId = createBorrow.Id,
                EquipmentItemId = item.Id
            };

            _context.BorrowItems.Add(borrowItem);
        }

        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<Object>()
        {
            Status = true,
            Message = "Borrow requested successfully",
            Data = equipmentItems.Select(e => new { e.Id })
        });
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("assign")]
    public async Task<IActionResult> AssignBorrow(AssignBorrowDto dto)
    {
        var equipment = await _context.Equipments.FindAsync(dto.EquipmentId);
        if (equipment == null)
            return BadRequest(new ApiResponse<Object>() { Status = false, Message = "Equipment not found", Data = null });

        var totalPrice = dto.BorrowedDays * dto.EquipmentCount * dto.EquipmentPrice;

        var borrow = new Borrow
        {
            UserId = dto.UserId,
            EquipmentId = dto.EquipmentId,
            EquipmentCount = dto.EquipmentCount,
            BorrowedDays = dto.BorrowedDays,
            EquipmentPrice = dto.EquipmentPrice,
            TotalPrice = totalPrice,
            DueAmount = totalPrice,
            Status = BorrowStatus.Assigned,
            AssingnedDate = DateTime.UtcNow
        };
        _context.Borrows.Add(borrow);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<Object>() { Status = true, Message = "Borrow assigned", Data = borrow });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("accept/{id:int}")]
    public async Task<IActionResult> AcceptBorrow(int id, AcceptedBorrowDto dto)
    {
        var borrow = await _context.Borrows.FindAsync(id);
        if (borrow == null || borrow.UserId != dto.UserId)
            return NotFound(new ApiResponse<Object>() { Status = false, Message = "Borrow not found", Data = null });

        borrow.Status = BorrowStatus.Accepted;
        borrow.AcceptedDate = DateTime.UtcNow;
        borrow.PostRemarks = dto.PostRemarks;
        borrow.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<Object>() { Status = true, Message = "Borrow accepted", Data = borrow });
    }

    [Authorize(Roles = "Client")]
    [HttpPut("pending/{id:int}")]
    public async Task<IActionResult> PendingBorrow(int id, PendingBorrowDto dto)
    {
        var borrow = await _context.Borrows.FindAsync(id);
        if (borrow == null || borrow.UserId != dto.UserId)
            return NotFound(new ApiResponse<Object>() { Status = false, Message = "Borrow not found", Data = null });

        borrow.Status = BorrowStatus.Pending;
        borrow.PendingDate = DateTime.UtcNow;
        borrow.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<Object>() { Status = true, Message = "Borrow marked pending", Data = borrow });
    }

    [Authorize(Roles = "Client")]
    [HttpPut("pay/{id:int}")]
    public async Task<IActionResult> PayBorrow(int id, PaidBorrowDto dto)
    {
        var borrow = await _context.Borrows.FindAsync(id);
        if (borrow == null || borrow.UserId != dto.UserId)
            return NotFound(new ApiResponse<Object>() { Status = false, Message = "Borrow not found", Data = (object?)null });

        borrow.PaymentMode = dto.PaymentMode;
        borrow.IsPaymentCompleted = dto.IsPaymentCompleted;
        borrow.PaymentId = dto.PaymentId;
        borrow.PaidAmount = borrow.TotalPrice;
        borrow.DueAmount = 0;
        borrow.Status = BorrowStatus.Paid;
        borrow.PaidDate = DateTime.UtcNow;
        borrow.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<Object>() { Status = true, Message = "Payment completed", Data = borrow });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("approve/{id:int}")]
    public async Task<IActionResult> ApproveBorrow(int id, ApprovedBorrowDto dto)
    {
        var borrow = await _context.Borrows.FindAsync(id);
        if (borrow == null || borrow.UserId != dto.UserId)
            return NotFound(new ApiResponse<Object>() { Status = false, Message = "Borrow not found", Data = null });
        if (!borrow.IsPaymentCompleted)
            return BadRequest(new ApiResponse<Object>() { Status = false, Message = "Payment is pending", Data = null });
        borrow.Status = BorrowStatus.Approved;
        borrow.ApprovedDate = DateTime.UtcNow;
        borrow.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { status = true, message = "Borrow approved", data = borrow });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("waitlist/{id:int}")]
    public async Task<IActionResult> WaitlistBorrow(int id, WaitlistedBorrowDto dto)
    {
        var borrow = await _context.Borrows.FindAsync(id);
        if (borrow == null || borrow.UserId != dto.UserId)
            return NotFound(new ApiResponse<Object>() { Status = false, Message = "Borrow not found", Data = (object?)null });

        borrow.Status = BorrowStatus.Waitlisted;
        borrow.WaitlistedDate = DateTime.UtcNow;
        borrow.PostRemarks = dto.PostRemarks;
        borrow.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<Object>() { Status = true, Message = "Borrow waitlisted", Data = borrow });
    }

    [Authorize(Roles = "Client")]
    [HttpPut("ack/{id:int}")]
    public async Task<IActionResult> AckBorrow(int id, AckBorrowDto dto)
    {
        var borrow = await _context.Borrows.FindAsync(id);
        if (borrow == null || borrow.UserId != dto.UserId)
            return NotFound(new ApiResponse<Object>() { Status = false, Message = "Borrow not found", Data = null });

        borrow.Status = BorrowStatus.Ack;
        borrow.AckDate = DateTime.UtcNow;
        borrow.AckRemarks = dto.AckRemarks;
        borrow.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<Object>() { Status = true, Message = "Borrow acknowledged", Data = borrow });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("close/{id:int}")]
    public async Task<IActionResult> CloseBorrow(int id, ClosedBorrowDto dto)
    {
        var borrow = await _context.Borrows.FindAsync(id);
        if (borrow == null || borrow.UserId != dto.UserId)
            return NotFound(new ApiResponse<Object>() { Status = false, Message = "Borrow not found", Data = (object?)null });

        borrow.Status = BorrowStatus.Closed;
        borrow.ClosedDate = DateTime.UtcNow;
        borrow.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<Object>() { Status = true, Message = "Borrow closed", Data = borrow });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _context.Borrows
            .Include(x => x.User)
            .Include(x => x.Equipment)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return Ok(new ApiResponse<Object>() { Status = true, Message = "Borrow list", Data = data });
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var borrow = await _context.Borrows
            .Include(x => x.User)
            .Include(x => x.Equipment)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (borrow == null)
            return NotFound(new ApiResponse<Object>() { Status = false, Message = "Borrow not found", Data = null });

        return Ok(new ApiResponse<Object>() { Status = true, Message = "Borrow details", Data = borrow });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var borrow = await _context.Borrows.FindAsync(id);
        if (borrow == null)
            return NotFound(new ApiResponse<Object>() { Status = false, Message = "Borrow not found", Data = (object?)null });

        _context.Borrows.Remove(borrow);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<Object>() { Status = true, Message = "Borrow deleted", Data = (object?)null });
    }
}
