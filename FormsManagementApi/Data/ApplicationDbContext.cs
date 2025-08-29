using Microsoft.EntityFrameworkCore;
using FormsManagementApi.Models;

namespace FormsManagementApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Department> Departments { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }
    public DbSet<Form> Forms { get; set; }
    public DbSet<FormSchemaVersion> FormSchemaVersions { get; set; }
    public DbSet<FormSubmission> FormSubmissions { get; set; }
    public DbSet<UsageMetric> UsageMetrics { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<SuperAdminUser> SuperAdminUsers { get; set; }
    public DbSet<WebhookEndpoint> WebhookEndpoints { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Department configuration
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            
            entity.HasOne(e => e.Department)
                .WithMany(e => e.Roles)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Permission configuration
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            
            entity.HasOne(e => e.Department)
                .WithMany(e => e.Permissions)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RolePermission configuration
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            
            entity.HasOne(e => e.Role)
                .WithMany(e => e.RolePermissions)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Permission)
                .WithMany(e => e.RolePermissions)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            
            entity.HasOne(e => e.Department)
                .WithMany(e => e.Users)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Role)
                .WithMany(e => e.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // UserPermission configuration
        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.PermissionId }).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            
            entity.HasOne(e => e.User)
                .WithMany(e => e.UserPermissions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Permission)
                .WithMany(e => e.UserPermissions)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Form configuration
        modelBuilder.Entity<Form>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            
            entity.HasOne(e => e.Department)
                .WithMany(e => e.Forms)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Creator)
                .WithMany(e => e.CreatedForms)
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // FormSchemaVersion configuration
        modelBuilder.Entity<FormSchemaVersion>(entity =>
        {
            entity.HasIndex(e => new { e.FormId, e.VersionNumber }).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            
            entity.HasOne(e => e.Form)
                .WithMany(e => e.FormSchemaVersions)
                .HasForeignKey(e => e.FormId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // FormSubmission configuration
        modelBuilder.Entity<FormSubmission>(entity =>
        {
            entity.Property(e => e.SubmittedAt).HasDefaultValueSql("now()");
            
            entity.HasOne(e => e.Form)
                .WithMany(e => e.FormSubmissions)
                .HasForeignKey(e => e.FormId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UsageMetric configuration
        modelBuilder.Entity<UsageMetric>(entity =>
        {
            entity.Property(e => e.RecordedAt).HasDefaultValueSql("now()");
            
            entity.HasOne(e => e.Department)
                .WithMany(e => e.UsageMetrics)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            
            entity.HasOne(e => e.Department)
                .WithMany(e => e.AuditLogs)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // SuperAdminUser configuration
        modelBuilder.Entity<SuperAdminUser>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
        });

        // WebhookEndpoint configuration
        modelBuilder.Entity<WebhookEndpoint>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            
            entity.HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed SuperAdmin user
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed SuperAdmin user
        modelBuilder.Entity<SuperAdminUser>().HasData(
            new SuperAdminUser
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Super Administrator",
                Email = "superadmin@system.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin123!"),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow
            }
        );
    }
}