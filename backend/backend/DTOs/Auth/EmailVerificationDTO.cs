using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Auth;

public class EmailVerificationDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string VerificationCode { get; set; } = string.Empty;
}
