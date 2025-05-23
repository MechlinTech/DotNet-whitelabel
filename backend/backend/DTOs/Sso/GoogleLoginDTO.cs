using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Auth;

public class GoogleLoginDTO
{
    [Required]
    public string IdToken { get; set; } = string.Empty;
}
