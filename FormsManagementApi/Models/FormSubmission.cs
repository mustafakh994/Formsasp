using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormsManagementApi.Models;

public class FormSubmission
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string SubmissionData { get; set; } = string.Empty; // JSON data
    
    [Required]
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    
    [MaxLength(45)]
    public string? IpAddress { get; set; }
    
    [MaxLength(500)]
    public string? UserAgent { get; set; }
    
    // Foreign keys
    [Required]
    public int FormId { get; set; }
    
    public int? UserId { get; set; }
    
    // Navigation properties
    [ForeignKey("FormId")]
    public virtual Form Form { get; set; } = null!;
    
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}