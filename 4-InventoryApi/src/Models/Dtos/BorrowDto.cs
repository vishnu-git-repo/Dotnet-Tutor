using Microsoft.EntityFrameworkCore;
using App.Models.Entities;

namespace App.Models.Dtos;

// Request
public class RequestBorrowDto
{
    public int UserId { get; set; }
    public string? Description {get; set;}
    public DateTime StartDate { get; set; }
    public DateTime ExpectedReturnDate { get; set; }
    public List<RequestBorrowItemDto> Items { get; set; } = new();
}
public class RequestBorrowItemDto
{
    public int EquipmentId { get; set; }
    public int Quantity { get; set; }
}


public class AssignBorrowDto
{
    public int UserId { get; set; }
    public string? Description {get; set;}
    public DateTime StartDate { get; set; }
    public DateTime ExpectedReturnDate { get; set; }
    public List<RequestBorrowItemDto> Items { get; set; } = new();
}

public class AcceptedBorrowDto
{
    public int UserId { get; set; }
    public string? Description { get; set; }
    public BorrowStatus Status { get; set; } = BorrowStatus.Accepted;
    public DateTime? AcceptedDate { get; set; }
}

public class PendingBorrowDto
{
    public int UserId { get; set; }
    public string? Description { get; set; }
}

public class PaidBorrowDto
{
    public int UserId { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public string? RazorpayOrderId { get; set; }
    public string? RazorpayPaymentId { get; set; }
    public string? RazorpaySignature { get; set; }
}

public class AdminCashPaymentDto
{
    public int UserId { get; set; }
    public decimal PaidAmount { get; set; }
    public string? Description { get; set; }
    public string? Remarks { get; set; }
}

public class ApprovedBorrowDto
{
    public int UserId { get; set; }
    public string? Description { get; set; }
    public BorrowStatus Status { get; set; } = BorrowStatus.Approved;
    public DateTime? ApprovedDate { get; set; }
}

public class WaitlistedBorrowDto
{
    public int UserId { get; set; }
    public string? Description { get; set; }
    public BorrowStatus Status { get; set; } = BorrowStatus.Waitlisted;
    public DateTime? WaitlistedDate { get; set; }
}

public class AckBorrowDto
{
    public int UserId { get; set; }
    public string? Description { get; set; }
    public BorrowStatus Status { get; set; } = BorrowStatus.Ack;
    public DateTime? AckDate { get; set; }
}

public class ClosedBorrowDto
{
    public int UserId { get; set; }
    public string? Description { get; set; }
    public BorrowStatus Status { get; set; } = BorrowStatus.Closed;
    public DateTime? ClosedDate { get; set; }
}


public class GetAdminBorrowDto
{
    public int PageNo {get; set;}
    public int RowCount {get; set;}
    public int Status {get; set;}
    public int? TotalCount {get; set;}
    public string SearchString {get; set;} = "";
    public int EquipmentId {get; set;}
    public int EquipmentItemId {get; set;}
    public int BorrowId {get; set;}
    public int UserId {get; set;}
}

public class GetClientBorrowDto
{
    public int PageNo {get; set;}
    public int RowCount {get; set;}
    public int Status {get; set;}
    public string SearchString {get; set;} = "";
    public int UserId {get; set;}
    public int? TotalCount {get; set;}
}

public class GetBorrowDto
{
    public int Id {get; set;}
}
