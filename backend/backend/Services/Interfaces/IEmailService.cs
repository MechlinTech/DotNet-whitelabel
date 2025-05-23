using backend.Models;

namespace backend.Services.Interfaces;

public interface IEmailService
{
    Task SendVerificationEmailAsync(ApplicationUser user, string verificationCode);
    Task SendPasswordResetEmailAsync(string email, string resetLink);
}
