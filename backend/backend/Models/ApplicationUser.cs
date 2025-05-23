using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace backend.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    // Tenant information
    public Guid? TenantId { get; set; }
    public string? TenantIdentifier { get; set; }

    [ForeignKey("TenantId")]
    public virtual Tenant? Tenant { get; set; }
}
