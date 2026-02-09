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
    public DbSet<Borrow> Borrows => Set<Borrow>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Equipment>(entity =>
        {
            entity.Property(e => e.Category).HasConversion<int>();
            entity.Property(e => e.Condition).HasConversion<int>();
            entity.Property(e => e.Status).HasConversion<int>();

            entity.Property(e => e.Price)
                  .HasPrecision(10, 2);

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(150);
        });
        builder.Entity<Borrow>( entity =>
        {
            entity.Property(b => b.Status).HasConversion<int>();
            entity.Property(b => b.PaymentMode).HasConversion<int>();
        });            

    }
}