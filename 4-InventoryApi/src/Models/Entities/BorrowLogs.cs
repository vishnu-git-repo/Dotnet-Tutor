using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace App.Models.Entities;

public class BorrowLogs
{
    [Key]
    public int Id {get; set;}

    public int UserId {get; set;}
    public int BorrowId {get; set;}
    public BorrowStatus Status {get; set;}
    public string Description {get; set;} ="";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [ForeignKey(nameof(BorrowId))]
    public Borrow? Borrow { get; set; }
}
