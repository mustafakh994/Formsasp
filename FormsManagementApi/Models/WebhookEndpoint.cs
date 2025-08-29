using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormsManagementApi.Models;

public class WebhookEndpoint
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Url { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Method { get; set; } = "POST";
    
    [MaxLength(1000)]
    public string? Headers { get; set; } // JSON string for headers
    
    [Required]
    public bool IsActive { get; set; } = true;
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Foreign key
    [Required]
    public int TenantId { get; set; }
    
    // Navigation property
    [ForeignKey("TenantId")]
    public virtual Tenant Tenant { get; set; } = null!;
}