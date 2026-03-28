using Microsoft.EntityFrameworkCore;
using VanSalesJourneyPlan.API.Models;

namespace VanSalesJourneyPlan.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<JourneyPlan> JourneyPlans { get; set; }
    public DbSet<JourneyPlanItem> JourneyPlanItems { get; set; }
    public DbSet<VisitLog> VisitLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Users table
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure Customers table
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId);
            entity.Property(e => e.CustomerCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.HasIndex(e => e.CustomerCode).IsUnique();
        });

        // Configure JourneyPlans table
        modelBuilder.Entity<JourneyPlan>(entity =>
        {
            entity.HasKey(e => e.JourneyPlanId);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.PlanDate).IsRequired();

            // Foreign keys
            entity.HasOne(e => e.AssignedUser)
                .WithMany(u => u.AssignedJourneyPlans)
                .HasForeignKey(e => e.AssignedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CreatedUser)
                .WithMany(u => u.CreatedJourneyPlans)
                .HasForeignKey(e => e.CreatedUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure JourneyPlanItems table
        modelBuilder.Entity<JourneyPlanItem>(entity =>
        {
            entity.HasKey(e => e.JourneyPlanItemId);
            entity.Property(e => e.SequenceNumber).IsRequired();

            // Foreign keys
            entity.HasOne(e => e.JourneyPlan)
                .WithMany(jp => jp.JourneyPlanItems)
                .HasForeignKey(e => e.JourneyPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Customer)
                .WithMany(c => c.JourneyPlanItems)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure VisitLogs table
        modelBuilder.Entity<VisitLog>(entity =>
        {
            entity.HasKey(e => e.VisitLogId);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.SalesAmount).HasPrecision(18, 2);

            // Foreign key
            entity.HasOne(e => e.JourneyPlanItem)
                .WithMany(jpi => jpi.VisitLogs)
                .HasForeignKey(e => e.JourneyPlanItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
