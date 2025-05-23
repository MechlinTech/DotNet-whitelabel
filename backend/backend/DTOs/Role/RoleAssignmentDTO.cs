using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Auth;

public class RoleAssignmentDTO
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string RoleName { get; set; } = string.Empty;
}
