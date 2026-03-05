using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using App.Models.Entities;

namespace App.Data;

public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Equipment> Equipments => Set<Equipment>();
    public DbSet<EquipmentItem> EquipmentItems => Set<EquipmentItem>();
    public DbSet<Borrow> Borrows => Set<Borrow>();
    public DbSet<BorrowItems> BorrowItems => Set<BorrowItems>();
    public DbSet<BorrowLogs> BorrowLogs => Set<BorrowLogs>();
    public DbSet<Payments> Payments => Set<Payments>();
    public DbSet<OTPHash> OTPHashes => Set<OTPHash>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Equipment>(entity =>
        {
            entity.Property(e => e.Category).HasConversion<int>();

            entity.Property(e => e.Price)
                  .HasPrecision(10, 2);

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(150);
        });

        builder.Entity<EquipmentItem>(entity =>
        {
            entity.Property(e => e.Condition).HasConversion<int>();
            entity.Property(e => e.Status).HasConversion<int>();
        });

        builder.Entity<Borrow>( entity =>
        {
            entity.Property(b => b.Status).HasConversion<int>();
        });            

    }
}