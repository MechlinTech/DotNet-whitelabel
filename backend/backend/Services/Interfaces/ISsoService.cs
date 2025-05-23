using backend.DTOs.Auth;
using backend.Models;

namespace backend.Services.Interfaces;

public interface ISsoService
{
    Task<(bool success, string message, string? token, string? email)> GoogleLoginAsync(string idToken);
    Task<(bool success, string message, string? token, string? email)> MicrosoftLoginAsync(string idToken);
}
