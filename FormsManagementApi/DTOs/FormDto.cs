using System.ComponentModel.DataAnnotations;

namespace FormsManagementApi.DTOs;

public class FormDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public object FormSchema { get; set; } = null!; // Will be deserialized JSON
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public int SubmissionCount { get; set; }
}

public class CreateFormDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    public object FormSchema { get; set; } = null!; // JSON object
    
    public bool IsActive { get; set; } = true;
}

public class UpdateFormDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    public object FormSchema { get; set; } = null!; // JSON object
    
    public bool IsActive { get; set; }
}

public class FormSubmissionDto
{
    public int Id { get; set; }
    public object SubmissionData { get; set; } = null!; // Will be deserialized JSON
    public DateTime SubmittedAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public int FormId { get; set; }
    public string FormName { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
}

public class CreateFormSubmissionDto
{
    [Required]
    public object SubmissionData { get; set; } = null!; // JSON object
    
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}

public class FormPermissionDto
{
    public int Id { get; set; }
    public string Permission { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int FormId { get; set; }
    public string FormName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
}

public class CreateFormPermissionDto
{
    [Required]
    public string Permission { get; set; } = string.Empty;
    
    [Required]
    public int UserId { get; set; }
}