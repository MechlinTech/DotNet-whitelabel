using backend.DTOs.Auth;
using backend.Models;

namespace backend.Services.Interfaces;

public interface IAuthService
{
    Task<(bool success, string message, string? token)> RegisterAsync(RegisterDTO model);
    Task<(bool success, string message, string? token)> LoginAsync(LoginDTO model);
    Task<(bool success, string message, string? token)> GoogleLoginAsync(string idToken);
    Task<(bool success, string message)> VerifyEmailAsync(EmailVerificationDTO model);
    Task<(bool success, string message)> RequestVerificationCodeAsync(string email);
    Task<(bool success, string message)> ForgotPasswordAsync(string email);
    Task<(bool success, string message)> ResetPasswordAsync(ResetPasswordDTO model);
    Task<string> GenerateJwtTokenAsync(ApplicationUser user);
}
