using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Auth;

public class MicrosoftLoginDTO
{
    [Required]
    public string IdToken { get; set; } = string.Empty;
}
