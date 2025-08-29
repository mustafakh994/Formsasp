using System.ComponentModel.DataAnnotations;
using System.Net;

namespace FormsManagementApi.DTOs;

public class FormDto
{
    public Guid Id { get; set; }
    public Guid DepartmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public object? FormSchema { get; set; } // JSON schema
    public object? Settings { get; set; } // JSON settings
    public Guid? CreatedBy { get; set; }
    public int Version { get; set; }
    public string? Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? DepartmentName { get; set; }
    public string? CreatorName { get; set; }
    public int SubmissionCount { get; set; }
}

public class CreateFormDto
{
    [Required]
    public Guid DepartmentId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Code { get; set; }
    
    [MaxLength(300)]
    public string? Title { get; set; }
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    public object? FormSchema { get; set; }
    
    public object? Settings { get; set; }
    
    [MaxLength(50)]
    public string? Status { get; set; }
}

public class UpdateFormDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Code { get; set; }
    
    [MaxLength(300)]
    public string? Title { get; set; }
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    public object? FormSchema { get; set; }
    
    public object? Settings { get; set; }
    
    [MaxLength(50)]
    public string? Status { get; set; }
}

public class FormSubmissionDto
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public object ResponseData { get; set; } = null!; // JSON response data
    public int? FormVersion { get; set; }
    public IPAddress? SubmitterIp { get; set; }
    public string? SubmitterEmail { get; set; }
    public DateTimeOffset SubmittedAt { get; set; }
    public string? FormName { get; set; }
}

public class CreateFormSubmissionDto
{
    [Required]
    public object ResponseData { get; set; } = null!; // JSON object
    
    public int? FormVersion { get; set; }
    
    public string? SubmitterIp { get; set; }
    
    [EmailAddress]
    public string? SubmitterEmail { get; set; }
}

public class FormSchemaVersionDto
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public int VersionNumber { get; set; }
    public object SchemaData { get; set; } = null!;
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatorName { get; set; }
}