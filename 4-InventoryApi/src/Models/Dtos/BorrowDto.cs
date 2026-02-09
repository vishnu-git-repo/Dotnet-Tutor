using Microsoft.EntityFrameworkCore;
using App.Models.Entities;

namespace App.Models.Dtos;

public class RequestBorrowDto
{
    public int UserId { get; set; }
    public int EquipmentId { get; set; }
    public int EquipmentCount { get; set; }
    public int BorrowedDays { get; set; }

    [Precision(18, 2)]
    public decimal EquipmentPrice { get; set; }

    public BorrowStatus Status { get; set; } = BorrowStatus.Requested;
    public DateTime? RequestedDate { get; set; }
}

public class AssignBorrowDto
{
    public int Id { get; set; }
    public string? PostRemarks { get; set; }

    public BorrowStatus Status { get; set; } = BorrowStatus.Assigned;
    public DateTime? AssingnedDate { get; set; }
}

public class AcceptedBorrowDto
{
    public int Id { get; set; }
    public string? PostRemarks { get; set; }

    public BorrowStatus Status { get; set; } = BorrowStatus.Accepted;
    public DateTime? AcceptedDate { get; set; }
}

public class PendingBorrowDto
{
    public int Id { get; set; }

    public BorrowStatus Status { get; set; } = BorrowStatus.Pending;
    public DateTime? PendingDate { get; set; }
}

public class PaidBorrowDto
{
    public int Id { get; set; }

    public PaymentMode PaymentMode { get; set; }
    public string? PaymentId { get; set; }
    public bool IsPaymentCompleted { get; set; } = true;

    public BorrowStatus Status { get; set; } = BorrowStatus.Paid;
    public DateTime? PaidDate { get; set; }
}

public class ApprovedBorrowDto
{
    public int Id { get; set; }

    public BorrowStatus Status { get; set; } = BorrowStatus.Approved;
    public DateTime? ApprovedDate { get; set; }
}

public class WaitlistedBorrowDto
{
    public int Id { get; set; }
    public string? PostRemarks { get; set; }

    public BorrowStatus Status { get; set; } = BorrowStatus.Waitlisted;
    public DateTime? WaitlistedDate { get; set; }
}

public class AckBorrowDto
{
    public int Id { get; set; }
    public string? AckRemarks { get; set; }

    public BorrowStatus Status { get; set; } = BorrowStatus.Ack;
    public DateTime? AckDate { get; set; }
}

public class ClosedBorrowDto
{
    public int Id { get; set; }

    public BorrowStatus Status { get; set; } = BorrowStatus.Closed;
    public DateTime? ClosedDate { get; set; }
}
