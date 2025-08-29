using System.ComponentModel.DataAnnotations;

namespace FormsManagementApi.Models;

public class Tenant
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public bool IsActive { get; set; } = true;
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Form> Forms { get; set; } = new List<Form>();
    public virtual ICollection<TenantSettings> TenantSettings { get; set; } = new List<TenantSettings>();
}