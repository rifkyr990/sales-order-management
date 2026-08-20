using Microsoft.EntityFrameworkCore;
using SalesOrderService.Models.Entities;

namespace SalesOrderService.Data;

public class SalesOrderDbContext : DbContext
{
    public SalesOrderDbContext(DbContextOptions<SalesOrderDbContext> options) : base(options) { }

    public DbSet<SalesSo> SalesSos => Set<SalesSo>();
    public DbSet<SalesSoLitem> SalesSoLitems => Set<SalesSoLitem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SalesSo>(entity =>
        {
            entity.HasKey(e => e.SalesSoId);
            entity.HasMany(e => e.Items)
                  .WithOne(i => i.SalesSo)
                  .HasForeignKey(i => i.SalesSoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SalesSoLitem>(entity =>
        {
            entity.HasKey(e => e.SalesSoLitemId);
        });
    }
}