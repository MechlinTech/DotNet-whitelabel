using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Tenant;

public class CreateTenantDTO
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Identifier { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }

    [Required]
    [StringLength(100)]
    public string Domain { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }
    public string? Theme { get; set; }
    public string? SubscriptionPlan { get; set; }
}

public class UpdateTenantDTO
{
    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    public bool? IsActive { get; set; }
    public string? LogoUrl { get; set; }
    public string? Theme { get; set; }
    public string? Domain { get; set; }
    public string? SubscriptionPlan { get; set; }
    public DateTime? SubscriptionExpiry { get; set; }
}

public class TenantResponseDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string DatabaseName { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Theme { get; set; }
    public string? Domain { get; set; }
    public string? SubscriptionPlan { get; set; }
    public DateTime? SubscriptionExpiry { get; set; }
}

public class AssignUserToTenantDTO
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public Guid TenantId { get; set; }
}
