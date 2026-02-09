using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.Models.Entities;
using App.Models.Dtos;
using App.Data;
using Microsoft.AspNetCore.Authorization;

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
            return BadRequest(new { status = false, message = "Equipment not found", data = (object?)null });

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
            Status = BorrowStatus.Requested,
            RequestedDate = DateTime.UtcNow
        };

        _context.Borrows.Add(borrow);
        await _context.SaveChangesAsync();

        return Ok(new { status = true, message = "Borrow requested", data = borrow });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("assign")]
    public async Task<IActionResult> AssignBorrow(AssignBorrowDto dto)
    {
        var borrow = await _context.Borrows.FindAsync(dto.Id);
        if (borrow == null)
            return NotFound(new { status = false, message = "Borrow not found", data = (object?)null });

        borrow.Status = BorrowStatus.Assigned;
        borrow.AssingnedDate = DateTime.UtcNow;
        borrow.PostRemarks = dto.PostRemarks;
        borrow.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { status = true, message = "Borrow assigned", data = borrow });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("accept/{id}")]
    public async Task<IActionResult> AcceptBorrow(string id, AcceptedBorrowDto dto)
    {
        var borrow = await _context.Borrows.FindAsync(int.Parse(id));
        if (borrow == null)
            return NotFound(new { status = false, message = "Borrow not found", data = (object?)null });

        borrow.Status = BorrowStatus.Accepted;
        borrow.AcceptedDate = DateTime.UtcNow;
        borrow.PostRemarks = dto.PostRemarks;
        borrow.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { status = true, message = "Borrow accepted", data = borrow });
    }

    [Authorize(Roles = "Client")]
    [HttpPut("pending/{id:int}")]
    public async Task<IActionResult> PendingBorrow(int id)
    {
        var borrow = await _context.Borrows.FindAsync(id);
        if (borrow == null)
            return NotFound(new { status = false, message = "Borrow not found", data = (object?)null });

        borrow.Status = BorrowStatus.Pending;
        borrow.PendingDate = DateTime.UtcNow;
        borrow.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { status = true, message = "Borrow marked pending", data = borrow });
    }

    [Authorize(Roles = "Client")]
    [HttpPut("pay/{id:int}")]
    public async Task<IActionResult> PayBorrow(int id, PaidBorrowDto dto)
    {
        var borrow = await _context.Borrows.FindAsync(id);
        if (borrow == null)
            return NotFound(new { status = false, message = "Borrow not found", data = (object?)null });

        borrow.PaymentMode = dto.PaymentMode;
        borrow.IsPaymentCompleted = dto.IsPaymentCompleted;
        borrow.PaymentId = dto.PaymentId;
        borrow.PaidAmount = borrow.TotalPrice;
        borrow.DueAmount = 0;
        borrow.Status = BorrowStatus.Paid;
        borrow.PaidDate = DateTime.UtcNow;
        borrow.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { status = true, message = "Payment completed", data = borrow });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("approve")]
    public async Task<IActionResult> ApproveBorrow(ApprovedBorrowDto dto)
    {
        var borrow = await _context.Borrows.FindAsync(dto.Id);
        if (borrow == null)
            return NotFound(new { status = false, message = "Borrow not found", data = (object?)null });

        borrow.Status = BorrowStatus.Approved;
        borrow.ApprovedDate = DateTime.UtcNow;
        borrow.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { status = true, message = "Borrow approved", data = borrow });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("waitlist")]
    public async Task<IActionResult> WaitlistBorrow(WaitlistedBorrowDto dto)
    {
        var borrow = await _context.Borrows.FindAsync(dto.Id);
        if (borrow == null)
            return NotFound(new { status = false, message = "Borrow not found", data = (object?)null });

        borrow.Status = BorrowStatus.Waitlisted;
        borrow.WaitlistedDate = DateTime.UtcNow;
        borrow.PostRemarks = dto.PostRemarks;
        borrow.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { status = true, message = "Borrow waitlisted", data = borrow });
    }

    [Authorize( Roles = "Client")]
    [HttpPut("ack")]
    public async Task<IActionResult> AckBorrow(AckBorrowDto dto)
    {
        var borrow = await _context.Borrows.FindAsync(dto.Id);
        if (borrow == null)
            return NotFound(new { status = false, message = "Borrow not found", data = (object?)null });

        borrow.Status = BorrowStatus.Ack;
        borrow.AckDate = DateTime.UtcNow;
        borrow.AckRemarks = dto.AckRemarks;
        borrow.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { status = true, message = "Borrow acknowledged", data = borrow });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("close")]
    public async Task<IActionResult> CloseBorrow(ClosedBorrowDto dto)
    {
        var borrow = await _context.Borrows.FindAsync(dto.Id);
        if (borrow == null)
            return NotFound(new { status = false, message = "Borrow not found", data = (object?)null });

        borrow.Status = BorrowStatus.Closed;
        borrow.ClosedDate = DateTime.UtcNow;
        borrow.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { status = true, message = "Borrow closed", data = borrow });
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

        return Ok(new { status = true, message = "Borrow list", data });
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
            return NotFound(new { status = false, message = "Borrow not found", data = (object?)null });

        return Ok(new { status = true, message = "Borrow details", data = borrow });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var borrow = await _context.Borrows.FindAsync(id);
        if (borrow == null)
            return NotFound(new { status = false, message = "Borrow not found", data = (object?)null });

        _context.Borrows.Remove(borrow);
        await _context.SaveChangesAsync();

        return Ok(new { status = true, message = "Borrow deleted", data = (object?)null });
    }
}
