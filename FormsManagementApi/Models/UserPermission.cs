using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormsManagementApi.Models;

public class UserPermission
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Permission { get; set; } = string.Empty;
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign key
    [Required]
    public int UserId { get; set; }
    
    // Navigation property
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}