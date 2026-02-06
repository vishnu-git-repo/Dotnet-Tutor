using Microsoft.EntityFrameworkCore;
using App.Models.Entities;
using System.Reflection.Metadata;

namespace App.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Equipment>(entity =>
        {
            entity.Property(e => e.Category).HasConversion<int>();
            entity.Property(e => e.Condition).HasConversion<int>();
            entity.Property(e => e.Status).HasConversion<int>();

            entity.Property(e => e.PricePerDay)
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