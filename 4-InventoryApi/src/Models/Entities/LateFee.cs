using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace App.Models.Entities;

public class LateFee
{
    [Key]
    public int Id {get; set;}

    public int UserId {get; set;}
    public int BorrowId {get; set;}

    public string Description {get; set;} = "";

    [Precision(18, 2)]
    public decimal Fee {get; set;}

    [ForeignKey(nameof(UserId))]
    public User? User {get; set;}

    [ForeignKey(nameof(BorrowId))]
    public Borrow? Borrow {get; set;}

    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
}