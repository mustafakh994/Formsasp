using Microsoft.EntityFrameworkCore;
using FormsManagementApi.Models;

namespace FormsManagementApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Form> Forms { get; set; }
    public DbSet<FormSubmission> FormSubmissions { get; set; }
    public DbSet<TenantSettings> TenantSettings { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }
    public DbSet<FormPermission> FormPermissions { get; set; }
    public DbSet<WebhookEndpoint> WebhookEndpoints { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tenant configuration
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(e => e.Tenant)
                .WithMany(e => e.Users)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Form configuration
        modelBuilder.Entity<Form>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(e => e.Tenant)
                .WithMany(e => e.Forms)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FormSubmission configuration
        modelBuilder.Entity<FormSubmission>(entity =>
        {
            entity.Property(e => e.SubmittedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(e => e.Form)
                .WithMany(e => e.FormSubmissions)
                .HasForeignKey(e => e.FormId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.User)
                .WithMany(e => e.FormSubmissions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // TenantSettings configuration
        modelBuilder.Entity<TenantSettings>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.Key }).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(e => e.Tenant)
                .WithMany(e => e.TenantSettings)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserPermission configuration
        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.Permission }).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(e => e.User)
                .WithMany(e => e.UserPermissions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FormPermission configuration
        modelBuilder.Entity<FormPermission>(entity =>
        {
            entity.HasIndex(e => new { e.FormId, e.UserId, e.Permission }).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(e => e.Form)
                .WithMany(e => e.FormPermissions)
                .HasForeignKey(e => e.FormId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // WebhookEndpoint configuration
        modelBuilder.Entity<WebhookEndpoint>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed SuperAdmin user
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed SuperAdmin user
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "Super Administrator",
                Email = "superadmin@system.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin123!"),
                Role = "SuperAdmin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                TenantId = null
            }
        );
    }
}