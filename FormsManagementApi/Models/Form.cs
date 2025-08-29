using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormsManagementApi.Models;

public class Form
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string FormSchema { get; set; } = string.Empty; // JSON schema for form fields
    
    [Required]
    public bool IsActive { get; set; } = true;
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Foreign key
    [Required]
    public int TenantId { get; set; }
    
    // Navigation properties
    [ForeignKey("TenantId")]
    public virtual Tenant Tenant { get; set; } = null!;
    
    public virtual ICollection<FormSubmission> FormSubmissions { get; set; } = new List<FormSubmission>();
    public virtual ICollection<FormPermission> FormPermissions { get; set; } = new List<FormPermission>();
}