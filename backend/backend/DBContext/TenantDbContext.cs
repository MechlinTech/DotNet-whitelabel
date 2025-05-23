using backend.Models.CRM;
using Microsoft.EntityFrameworkCore;

namespace backend.DBContext;

public class TenantDbContext : DbContext
{
    public TenantDbContext(DbContextOptions<TenantDbContext> options)
        : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Deal> Deals { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // CRM configurations
        builder.Entity<Customer>(entity =>
        {
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.Status);
        });

        builder.Entity<Contact>(entity =>
        {
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.Email);
        });

        builder.Entity<Deal>(entity =>
        {
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.Stage);
        });
    }
}
