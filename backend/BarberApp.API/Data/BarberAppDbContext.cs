using Microsoft.EntityFrameworkCore;
using BarberApp.API.Models;

namespace BarberApp.API.Data;

public class BarberAppDbContext : DbContext
{
    public BarberAppDbContext(DbContextOptions<BarberAppDbContext> options)
        : base(options)
    {
    }

    public DbSet<BarberInfo> BarberInfos { get; set; }
    public DbSet<PortfolioItem> PortfolioItems { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<ServiceType> ServiceTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BarberInfo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Bio).HasMaxLength(1000);
        });

        modelBuilder.Entity<PortfolioItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CustomerPhone).HasMaxLength(20);
            entity.Property(e => e.ServiceType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(500);
        });

        modelBuilder.Entity<ServiceType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
        });

        // Seed default data
        modelBuilder.Entity<ServiceType>().HasData(
            new ServiceType { Id = 1, Name = "Haircut", Description = "Classic haircut", Price = 25.00m, DurationMinutes = 30, IsActive = true },
            new ServiceType { Id = 2, Name = "Beard Trim", Description = "Beard trimming and styling", Price = 15.00m, DurationMinutes = 20, IsActive = true },
            new ServiceType { Id = 3, Name = "Hair & Beard", Description = "Complete haircut and beard trim", Price = 35.00m, DurationMinutes = 45, IsActive = true }
        );

        modelBuilder.Entity<BarberInfo>().HasData(
            new BarberInfo 
            { 
                Id = 1, 
                Name = "John's Barber Shop", 
                Bio = "Professional barbering services with years of experience. Quality cuts and excellent customer service.",
                Email = "john@barbershop.com",
                Phone = "(555) 123-4567",
                Address = "123 Main Street, City, State 12345",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}
