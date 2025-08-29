using System.ComponentModel.DataAnnotations;

namespace FormsManagementApi.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? TenantId { get; set; }
    public string? TenantName { get; set; }
    public List<string> Permissions { get; set; } = new();
}

public class CreateUserDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Role { get; set; } = "User";
    
    public bool IsActive { get; set; } = true;
    
    public int? TenantId { get; set; }
    
    public List<string> Permissions { get; set; } = new();
}

public class UpdateUserDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty;
    
    public bool IsActive { get; set; }
    
    public int? TenantId { get; set; }
    
    public List<string> Permissions { get; set; } = new();
}

public class UserPermissionDto
{
    public int Id { get; set; }
    public string Permission { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}