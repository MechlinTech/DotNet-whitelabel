using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class Tenant
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Identifier { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Database configuration
    [Required]
    public string DatabaseName { get; set; } = string.Empty;

    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    // Tenant-specific settings
    public string? LogoUrl { get; set; }
    public string? Theme { get; set; }
    public string? Domain { get; set; }
    public string? SubscriptionPlan { get; set; }
    public DateTime? SubscriptionExpiry { get; set; }

    // Navigation properties
    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}
