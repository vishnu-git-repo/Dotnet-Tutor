using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.Models.Entities;
using App.Models.Dtos;
using App.Data;
using Microsoft.AspNetCore.Authorization;
using App.Common;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("api/borrows")]
public class BorrowController : ControllerBase
{
    private readonly AppDBContext _context;
    private readonly IConfiguration _configuration;

    public BorrowController(AppDBContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    private bool VerifySignature(string orderId, string paymentId, string signature)
    {
        var secret = _configuration["Razorpay:Secret"];
        if (string.IsNullOrEmpty(secret)) return false;

        var payload = $"{orderId}|{paymentId}";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var generatedSignature = BitConverter.ToString(hash)
            .Replace("-", "")
            .ToLower();

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(generatedSignature),
            Encoding.UTF8.GetBytes(signature)
        );
    }

    [Authorize(Roles = "Client")]
    [HttpPost("request")]
    public async Task<IActionResult> RequestBorrow(RequestBorrowDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            if (dto.Items == null || !dto.Items.Any())
            {
                return Ok(new ApiResponse<object>
                {
                    Status = false,
                    Message = "No items selected",
                    Data = null
                });
            }

            var createBorrow = new Borrow
            {
                UserId = dto.UserId,
                StartDate = dto.StartDate,
                ExpectedReturnDate = dto.ExpectedReturnDate,
                RequestedDate = DateTime.UtcNow,
                Status = BorrowStatus.Requested
            };

            _context.Borrows.Add(createBorrow);
            await _context.SaveChangesAsync();

            decimal totalPrice = 0;

            foreach (var requestItem in dto.Items)
            {
                var equipment = await _context.Equipments
                    .FirstOrDefaultAsync(e => e.Id == requestItem.EquipmentId);

                if (equipment == null)
                {
                    await transaction.RollbackAsync();
                    return Ok(new ApiResponse<object>
                    {
                        Status = false,
                        Message = $"Equipment {requestItem.EquipmentId} not found",
                        Data = null
                    });
                }

                var availableItems = await _context.EquipmentItems
                    .Where(e => e.EquipmentId == requestItem.EquipmentId &&
                                e.Status == EquipmentStatus.Available)
                    .Take(requestItem.Quantity)
                    .ToListAsync();

                if (availableItems.Count < requestItem.Quantity)
                {
                    await transaction.RollbackAsync();
                    return Ok(new ApiResponse<object>
                    {
                        Status = false,
                        Message = $"Not enough stock for {equipment.Name}",
                        Data = null
                    });
                }

                foreach (var item in availableItems)
                {
                    item.Status = EquipmentStatus.InUse;

                    _context.BorrowItems.Add(new BorrowItems
                    {
                        BorrowId = createBorrow.Id,
                        EquipmentId = equipment.Id,
                        EquipmentItemId = item.Id,
                        EquipmentPrice = equipment.Price
                    });

                    totalPrice += equipment.Price;
                }
            }

            createBorrow.TotalPrice = totalPrice;
            createBorrow.DueAmount = totalPrice;
            createBorrow.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new ApiResponse<object>
            {
                Status = true,
                Message = "Borrow requested successfully",
                Data = new { BorrowId = createBorrow.Id }
            });
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();

            return Ok(new ApiResponse<object>
            {
                Status = false,
                Message = "Something went wrong",
                Data = null
            });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("assign")]
    public async Task<IActionResult> AssignBorrow(AssignBorrowDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            if (dto.Items == null || !dto.Items.Any())
            {
                return Ok(new ApiResponse<object>
                {
                    Status = false,
                    Message = "No items selected",
                    Data = null
                });
            }

            var createBorrow = new Borrow
            {
                UserId = dto.UserId,
                StartDate = dto.StartDate,
                ExpectedReturnDate = dto.ExpectedReturnDate,
                AssignedDate = DateTime.UtcNow,
                Status = BorrowStatus.Assigned
            };

            _context.Borrows.Add(createBorrow);
            await _context.SaveChangesAsync();

            decimal totalPrice = 0;

            foreach (var requestItem in dto.Items)
            {
                var equipment = await _context.Equipments
                    .FirstOrDefaultAsync(e => e.Id == requestItem.EquipmentId);

                if (equipment == null)
                {
                    await transaction.RollbackAsync();
                    return Ok(new ApiResponse<object>
                    {
                        Status = false,
                        Message = $"Equipment {requestItem.EquipmentId} not found",
                        Data = null
                    });
                }

                var availableItems = await _context.EquipmentItems
                    .Where(e => e.EquipmentId == requestItem.EquipmentId &&
                                e.Status == EquipmentStatus.Available)
                    .Take(requestItem.Quantity)
                    .ToListAsync();

                if (availableItems.Count < requestItem.Quantity)
                {
                    await transaction.RollbackAsync();
                    return Ok(new ApiResponse<object>
                    {
                        Status = false,
                        Message = $"Not enough stock for {equipment.Name}",
                        Data = null
                    });
                }

                foreach (var item in availableItems)
                {
                    item.Status = EquipmentStatus.InUse;

                    _context.BorrowItems.Add(new BorrowItems
                    {
                        BorrowId = createBorrow.Id,
                        EquipmentId = equipment.Id,
                        EquipmentItemId = item.Id,
                        EquipmentPrice = equipment.Price
                    });

                    totalPrice += equipment.Price;
                }
            }

            createBorrow.TotalPrice = totalPrice;
            createBorrow.DueAmount = totalPrice;
            createBorrow.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new ApiResponse<object>
            {
                Status = true,
                Message = "Borrow requested successfully",
                Data = new { BorrowId = createBorrow.Id }
            });
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();

            return Ok(new ApiResponse<object>
            {
                Status = false,
                Message = "Something went wrong",
                Data = null
            });
        }
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
        borrow.PreRemarks = dto.PreRemarks;
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

    [Authorize(Roles = "Admin")]
    [HttpPut("admin/collect-cash/{id:int}")]
    public async Task<IActionResult> CollectCash(int id, AdminCashPaymentDto dto)
    {
        var borrow = await _context.Borrows.FindAsync(id);

        if (borrow == null || borrow.UserId != dto.UserId)
            return NotFound(new ApiResponse<object>
            {
                Status = false,
                Message = "Borrow not found",
                Data = null
            });

        if (borrow.IsPaymentCompleted)
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Payment already completed",
                Data = null
            });

        if (borrow.Status != BorrowStatus.Pending)
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Borrow must be in pending state",
                Data = null
            });

        if (dto.PaidAmount <= 0)
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Invalid paid amount",
                Data = null
            });

        if (dto.PaidAmount != borrow.TotalPrice)
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Paid amount must match total price",
                Data = null
            });

        borrow.PaymentMode = PaymentMode.Cash;
        borrow.IsPaymentCompleted = true;
        borrow.PaymentCompletedDate = DateTime.UtcNow;

        borrow.PaidAmount = dto.PaidAmount;
        borrow.DueAmount = 0;

        borrow.Status = BorrowStatus.Paid;
        borrow.PaidDate = DateTime.UtcNow;

        borrow.PostRemarks = dto.Remarks;
        borrow.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            Status = true,
            Message = "Cash collected successfully",
            Data = borrow
        });
    }


    [Authorize(Roles = "Client")]
    [HttpPut("pay/{id:int}")]
    public async Task<IActionResult> PayBorrow(int id, PaidBorrowDto dto)
    {
        var borrow = await _context.Borrows.FindAsync(id);

        if (borrow == null || borrow.UserId != dto.UserId)
            return NotFound(new ApiResponse<object>
            {
                Status = false,
                Message = "Borrow not found",
                Data = null
            });

        if (borrow.IsPaymentCompleted)
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Payment already completed",
                Data = null
            });

        if (borrow.Status != BorrowStatus.Pending)
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Borrow is not in pending state",
                Data = null
            });

        if (borrow.RazorpayOrderId != dto.RazorpayOrderId)
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Order ID mismatch",
                Data = null
            });

        if (!VerifySignature(dto.RazorpayOrderId!, dto.RazorpayPaymentId!, dto.RazorpaySignature!))
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Invalid payment signature",
                Data = null
            });

        borrow.PaymentMode = PaymentMode.RazorPay;

        borrow.RazorpayPaymentId = dto.RazorpayPaymentId;
        borrow.RazorpaySignature = dto.RazorpaySignature;

        borrow.IsPaymentCompleted = true;
        borrow.PaymentCompletedDate = DateTime.UtcNow;
        borrow.PaidAmount = borrow.TotalPrice;
        borrow.DueAmount = 0;

        borrow.Status = BorrowStatus.Paid;
        borrow.PaidDate = DateTime.UtcNow;
        borrow.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            Status = true,
            Message = "Payment completed successfully",
            Data = borrow
        });
    }

    [Authorize(Roles = "Client")]
    [HttpPost("create-order/{id:int}")]
    public async Task<IActionResult> CreateOrder(int id)
    {
        var borrow = await _context.Borrows.FindAsync(id);

        if (borrow == null)
            return NotFound(new ApiResponse<object>
            {
                Status = false,
                Message = "Borrow not found",
                Data = null
            });

        if (borrow.Status != BorrowStatus.Pending)
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Borrow not in pending state",
                Data = null
            });

        if (borrow.IsPaymentCompleted)
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Payment already completed",
                Data = null
            });

        if (!string.IsNullOrEmpty(borrow.RazorpayOrderId))
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Order already created",
                Data = null
            });

        var key = _configuration["Razorpay:Key"];
        var secret = _configuration["Razorpay:Secret"];

        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(secret))
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Razorpay configuration missing",
                Data = null
            });

        RazorpayClient client = new RazorpayClient(key, secret);

        var options = new Dictionary<string, object>
        {
            { "amount", (int)(borrow.TotalPrice * 100) },
            { "currency", "INR" },
            { "receipt", $"borrow_{borrow.Id}" },
            { "payment_capture", 1 }
        };

        Order order = client.Order.Create(options);

        borrow.RazorpayOrderId = order["id"].ToString();
        borrow.PaymentInitiatedDate = DateTime.UtcNow;
        borrow.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            Status = true,
            Message = "Order created",
            Data = new
            {
                orderId = borrow.RazorpayOrderId,
                amount = options["amount"],
                key = key
            }
        });
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
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return Ok(new ApiResponse<Object>() { Status = true, Message = "Borrow list", Data = data });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("adminBorrows")]
    public async Task<IActionResult> GetAdminBorrows(GetAdminBorrowDto dto)
    {
        try
        {
            var baseQuery = _context.Borrows.AsNoTracking();

            var totalCount = await baseQuery.CountAsync();

            if (dto.Status != 0)
            {
                baseQuery = baseQuery.Where(b => b.Status == (BorrowStatus)dto.Status);
            }

            if (!string.IsNullOrWhiteSpace(dto.SearchString))
            {
                var search = dto.SearchString.ToLower();

                baseQuery = baseQuery.Where(b =>
                    (b.User != null && b.User.Name.ToLower().Contains(search)) ||
                    b.BorrowItems!.Any(bi =>
                        bi.Equipment != null &&
                        bi.Equipment.Name.ToLower().Contains(search))
                );
            }

            if (dto.EquipmentId != 0)
            {
                baseQuery = baseQuery.Where(b =>
                    b.BorrowItems!.Any(bi =>
                        bi.EquipmentId == dto.EquipmentId));
            }

            if (dto.EquipmentItemId != 0)
            {
                baseQuery = baseQuery.Where(b =>
                    b.BorrowItems!.Any(bi =>
                        bi.EquipmentItemId == dto.EquipmentItemId));
            }

            if (dto.UserId != 0)
            {
                baseQuery = baseQuery.Where(b => b.UserId == dto.UserId);
            }

            if (dto.BorrowId != 0)
            {
                baseQuery = baseQuery.Where(b => b.Id == dto.BorrowId);
            }

            var filteredCount = await baseQuery.CountAsync();

            var statusCounts = await _context.Borrows
                .GroupBy(b => b.Status)
                .Select(g => new
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.Status, x => x.Count);

            if ((dto.PageNo != 0) && (dto.RowCount != 0))
            {
                baseQuery = baseQuery
                    .OrderByDescending(b => b.CreatedAt)
                    .Skip((dto.PageNo - 1) * dto.RowCount)
                    .Take(dto.RowCount);
            }
            else
            {
                baseQuery = baseQuery.OrderByDescending(b => b.CreatedAt);
            }

            var data = await baseQuery
                .Select(b => new
                {
                    b.Id,
                    b.Status,
                    b.CreatedAt,
                    b.EquipmentCounts,
                    b.StartDate,
                    b.ExpectedReturnDate,
                    b.ActualReturnDate,
                    b.TotalPrice,
                    b.DueAmount,
                    b.UserId,
                    UserName = b.User != null ? b.User.Name : "",
                    Equipments = b.BorrowItems!
                        .Select(bi => new
                        {
                            bi.EquipmentId,
                            EquipmentName = bi.Equipment != null ? bi.Equipment.Name : ""
                        })
                })
                .ToListAsync();

            var result = data.Select(b => new
            {
                b.Id,
                b.Status,
                b.CreatedAt,
                b.EquipmentCounts,
                b.StartDate,
                b.ExpectedReturnDate,
                b.ActualReturnDate,
                ExpectedDuration = DateHelper.CalculateDuration(b.StartDate, b.ExpectedReturnDate),
                b.TotalPrice,
                b.DueAmount,
                b.UserId,
                b.UserName,
                b.Equipments
            });

            return Ok(new ApiResponse<object>
            {
                Status = true,
                Message = "Borrow fetched successfully",
                Data = new
                {
                    TotalCount = totalCount,
                    FilteredCount = filteredCount,
                    StatusCounts = statusCounts,
                    Items = result
                }
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.InnerException);
            return Ok(new ApiResponse<object>
            {
                Status = false,
                Message = e.Message,
                Data = null
            });
        }
    }


    [Authorize(Roles = "Client")]
    [HttpPost("clientBorrows")]
    public async Task<IActionResult> GetClientBorrows(GetClientBorrowDto dto)
    {
        try
        {
            var baseQuery = _context.Borrows.AsNoTracking();

            if (dto.UserId != 0)
            {
                baseQuery = baseQuery.Where(b => b.UserId == dto.UserId);
            }
            var totalCount = await baseQuery.CountAsync();
            var statusCounts = await baseQuery
                .GroupBy(b => b.Status)
                .Select(g => new
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.Status, x => x.Count);


            if (dto.Status != 0)
            {
                baseQuery = baseQuery.Where(b => b.Status == (BorrowStatus)dto.Status);
            }

            if (!string.IsNullOrWhiteSpace(dto.SearchString))
            {
                var search = dto.SearchString.ToLower();

                baseQuery = baseQuery.Where(b =>
                    (b.User != null && b.User.Name.ToLower().Contains(search)) ||
                    b.BorrowItems!.Any(bi =>
                        bi.Equipment != null &&
                        bi.Equipment.Name.ToLower().Contains(search))
                );
            }

            var filteredCount = await baseQuery.CountAsync();

            if ((dto.PageNo != 0) && (dto.RowCount != 0))
            {
                baseQuery = baseQuery
                    .OrderByDescending(b => b.CreatedAt)
                    .Skip((dto.PageNo - 1) * dto.RowCount)
                    .Take(dto.RowCount);
            }
            else
            {
                baseQuery = baseQuery.OrderByDescending(b => b.CreatedAt);
            }

            var data = await baseQuery
                .Select(b => new
                {
                    b.Id,
                    b.Status,
                    b.CreatedAt,
                    b.EquipmentCounts,
                    b.StartDate,
                    b.ExpectedReturnDate,
                    b.ActualReturnDate,
                    b.TotalPrice,
                    b.DueAmount,
                    b.UserId,
                    UserName = b.User != null ? b.User.Name : "",
                    Equipments = b.BorrowItems!
                        .Select(bi => new
                        {
                            bi.EquipmentId,
                            EquipmentName = bi.Equipment != null ? bi.Equipment.Name : ""
                        })
                })
                .ToListAsync();

            var result = data.Select(b => new
            {
                b.Id,
                b.Status,
                b.CreatedAt,
                b.EquipmentCounts,
                b.StartDate,
                b.ExpectedReturnDate,
                b.ActualReturnDate,
                ExpectedDuration = DateHelper.CalculateDuration(b.StartDate, b.ExpectedReturnDate),
                b.TotalPrice,
                b.DueAmount,
                b.UserId,
                b.UserName,
                b.Equipments
            });


            return Ok(new ApiResponse<Object>
            {
                Status = true,
                Message = "Borrow Fetched Successfully",
                Data = new
                {
                    TotalCount = totalCount,
                    FilteredCount = filteredCount,
                    StatusCounts = statusCounts,
                    Items = result
                }
            });
        }
        catch (Exception)
        {
            return StatusCode(500, ErrorResponse());
        }
    }


    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBorrow(int id)
    {
        try
        {
            var borrow = await _context.Borrows
            .Include(x => x.User)
            .Select(b => new
            {
                // User
                userId = b.User!.Id,
                userName = b.User!.Name,
                userEmail = b.User!.Email,

                // Borrow
                borrowId = b.Id,
                borrowStatus = b.Status,
                b.StartDate,
                b.ExpectedReturnDate,
                b.ActualReturnDate,
                b.EquipmentCounts,

                // Price
                b.TotalPrice,
                b.PaidAmount,
                b.DueAmount,
                b.LateFee,

                // Payment
                b.PaymentMode,
                b.IsPaymentCompleted,
                b.RazorpayPaymentId,

                // Simple Logs
                b.RequestedDate,
                b.AcceptedDate,
                b.AssignedDate,
                b.PendingDate,
                b.PaidDate,
                b.ApprovedDate,
                b.WaitlistedDate,
                b.AckDate,
                b.ClosedDate,

                // Remarks or Description
                b.PreRemarks,
                b.PostRemarks,
                b.AckRemarks,

                // Time Stamp
                b.CreatedAt,
                b.UpdatedAt
            })
            .FirstOrDefaultAsync(b => b.borrowId == id);
            if (borrow == null)
                return NotFound(new ApiResponse<Object>()
                {
                    Status = false,
                    Message = "Borrow not found",
                    Data = null
                });

            var borrowItems = await _context.BorrowItems
                .Where(bi => bi.BorrowId == id)
                .Include(bi => bi.Equipment)
                .Include(bi => bi.EquipmentItem)
                .Select(bi => new
                {
                    // Equipment
                    equipmentId = bi.EquipmentId,
                    equipmentDescription = bi.Equipment!.Description,
                    equipmentName = bi.Equipment!.Name,
                    equipmentItemId = bi.EquipmentItemId,
                    borrowedPrice = bi.EquipmentPrice,
                    bi.IsReturned,
                })
                .ToListAsync();
            if (borrowItems == null)
            {
                return NotFound(new ApiResponse<Object>()
                {
                    Status = false,
                    Message = "Borrow Items not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<Object>
            {
                Status = true,
                Message = "Borrow Fetched Successfully",
                Data =
                new
                {
                    Borrow = borrow,
                    Equipments = borrowItems
                }
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.InnerException);
            return Ok(new ApiResponse<object>
            {
                Status = false,
                Message = e.Message,
                Data = null
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
