using System.Net;
using System.Net.Mail;
using backend.Models;
using backend.Services.Interfaces;

namespace backend.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendVerificationEmailAsync(ApplicationUser user, string verificationCode)
    {
        var subject = "Verify your email address";
        var body =
            $@"
            <h2>Welcome to our application!</h2>
            <p>Please use the following code to verify your email address:</p>
            <h1 style='color: #4CAF50;'>{verificationCode}</h1>
            <p>This code will expire in 10 minutes.</p>
            <p>If you didn't request this verification, please ignore this email.</p>";

        await SendEmailAsync(user.Email!, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetLink)
    {
        var subject = "Reset your password";
        var body =
            $@"
            <h2>Password Reset Request</h2>
            <p>Please click the link below to reset your password:</p>
            <p><a href='{resetLink}'>Reset Password</a></p>
            <p>If you didn't request a password reset, please ignore this email.</p>";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpServer =
            _configuration["EmailSettings:SmtpServer"]
            ?? throw new InvalidOperationException("SMTP server is not configured");
        var smtpPort = int.Parse(
            _configuration["EmailSettings:SmtpPort"]
                ?? throw new InvalidOperationException("SMTP port is not configured")
        );
        var smtpUsername =
            _configuration["EmailSettings:SmtpUsername"]
            ?? throw new InvalidOperationException("SMTP username is not configured");
        var smtpPassword =
            _configuration["EmailSettings:SmtpPassword"]
            ?? throw new InvalidOperationException("SMTP password is not configured");
        var fromEmail =
            _configuration["EmailSettings:FromEmail"]
            ?? throw new InvalidOperationException("From email is not configured");
        var fromName =
            _configuration["EmailSettings:FromName"]
            ?? throw new InvalidOperationException("From name is not configured");

        using var client = new SmtpClient(smtpServer, smtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(smtpUsername, smtpPassword),
        };

        using var message = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        message.To.Add(to);
        await client.SendMailAsync(message);
    }
}
