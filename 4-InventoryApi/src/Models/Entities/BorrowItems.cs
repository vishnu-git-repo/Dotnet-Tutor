using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace App.Models.Entities;

public class BorrowItems
{
    public int Id {get; set;}
    public int EquipmentItemId {get; set;}
    public int BorrowId {get; set;}
}