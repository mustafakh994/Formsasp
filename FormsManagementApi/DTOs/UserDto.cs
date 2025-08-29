using System.ComponentModel.DataAnnotations;

namespace FormsManagementApi.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public Guid DepartmentId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public Guid? RoleId { get; set; }
    public object? Profile { get; set; } // JSON profile data
    public object? CustomPermissions { get; set; } // JSON custom permissions
    public bool IsActive { get; set; }
    public DateTimeOffset? EmailVerifiedAt { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? DepartmentName { get; set; }
    public string? RoleName { get; set; }
    public List<PermissionDto> Permissions { get; set; } = new();
}

public class CreateUserDto
{
    [Required]
    public Guid DepartmentId { get; set; }
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Name { get; set; }
    
    public Guid? RoleId { get; set; }
    
    public object? Profile { get; set; }
    
    public object? CustomPermissions { get; set; }
    
    public bool IsActive { get; set; } = true;
}

public class UpdateUserDto
{
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Name { get; set; }
    
    public Guid? RoleId { get; set; }
    
    public object? Profile { get; set; }
    
    public object? CustomPermissions { get; set; }
    
    public bool IsActive { get; set; }
}

public class UserPermissionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }
    public bool Granted { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? PermissionName { get; set; }
}