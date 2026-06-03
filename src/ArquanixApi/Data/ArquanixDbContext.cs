using ArquanixApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ArquanixApi.Data;


public class ArquanixDbContext : DbContext
{
    public ArquanixDbContext(DbContextOptions<ArquanixDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();

    public DbSet<Claim> Claims => Set<Claim>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Email).IsRequired().HasMaxLength(256);
            entity.Property(c => c.Phone).HasMaxLength(40);
        });

        modelBuilder.Entity<Claim>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Title).IsRequired().HasMaxLength(120);
            entity.Property(c => c.Description).IsRequired().HasMaxLength(1000);
            entity.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(c => c.Priority).HasConversion<string>().HasMaxLength(20);

            entity.HasOne<Client>()
                .WithMany()
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
