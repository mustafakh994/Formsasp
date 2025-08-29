using System.ComponentModel.DataAnnotations;

namespace FormsManagementApi.DTOs;

public class WebhookEndpointDto
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public object? Headers { get; set; } // Will be deserialized JSON
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
}

public class CreateWebhookEndpointDto
{
    [Required]
    [MaxLength(500)]
    [Url]
    public string Url { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Method { get; set; } = "POST";
    
    public object? Headers { get; set; } // JSON object for headers
    
    public bool IsActive { get; set; } = true;
}

public class UpdateWebhookEndpointDto
{
    [Required]
    [MaxLength(500)]
    [Url]
    public string Url { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Method { get; set; } = "POST";
    
    public object? Headers { get; set; } // JSON object for headers
    
    public bool IsActive { get; set; }
}