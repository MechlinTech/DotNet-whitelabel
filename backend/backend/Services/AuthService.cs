using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.DTOs.Auth;
using backend.Models;
using backend.Services.Interfaces;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        IEmailService emailService,
        ILogger<AuthService> logger
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<(bool success, string message, string? token)> RegisterAsync(
        RegisterDTO model
    )
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            FullName = $"{model.FirstName} {model.LastName}",
            EmailConfirmed = false,
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)), null);
        }

        // Create User role if it doesn't exist
        if (!await _roleManager.RoleExistsAsync("User"))
        {
            await _roleManager.CreateAsync(new IdentityRole("User"));
        }

        // Add user to default role
        await _userManager.AddToRoleAsync(user, "User");

        // Generate verification code and send email
        var verificationCode = GenerateVerificationCode();
        await _emailService.SendVerificationEmailAsync(user, verificationCode);

        // Store verification code in user claims
        await _userManager.AddClaimAsync(user, new Claim("VerificationCode", verificationCode));
        await _userManager.AddClaimAsync(
            user,
            new Claim("VerificationCodeExpiry", DateTime.UtcNow.AddMinutes(10).ToString("O"))
        );

        var token = await GenerateJwtTokenAsync(user);
        return (true, "User registered successfully", token);
    }

    public async Task<(bool success, string message, string? token)> LoginAsync(LoginDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return (false, "Invalid email or password", null);
        }

        var result = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!result)
        {
            return (false, "Invalid email or password", null);
        }

        var token = await GenerateJwtTokenAsync(user);
        return (true, "Login successful", token);
    }

    public async Task<(bool success, string message, string? token)> GoogleLoginAsync(
        string idToken
    )
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _configuration["Authentication:Google:ClientId"] },
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user == null)
            {
                // Register new user
                user = new ApplicationUser
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    FirstName = payload.GivenName ?? "Google",
                    LastName = payload.FamilyName ?? "User",
                    FullName = payload.Name,
                    EmailConfirmed = true,
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return (
                        false,
                        string.Join(", ", result.Errors.Select(e => e.Description)),
                        null
                    );
                }

                await _userManager.AddToRoleAsync(user, "User");
            }
            else if (user.PasswordHash != null)
            {
                return (false, "This email is already registered. Please use regular login.", null);
            }

            var token = await GenerateJwtTokenAsync(user);
            return (true, "Google login successful", token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Google login failed");
            return (false, "Google authentication failed", null);
        }
    }

    public async Task<(bool success, string message)> VerifyEmailAsync(EmailVerificationDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return (false, "User not found");
        }

        var claims = await _userManager.GetClaimsAsync(user);
        var codeClaim = claims.FirstOrDefault(c => c.Type == "VerificationCode");
        var expiryClaim = claims.FirstOrDefault(c => c.Type == "VerificationCodeExpiry");

        if (codeClaim == null || expiryClaim == null)
        {
            return (false, "No verification code found");
        }

        if (DateTime.Parse(expiryClaim.Value) < DateTime.UtcNow)
        {
            return (false, "Verification code has expired");
        }

        if (codeClaim.Value != model.VerificationCode)
        {
            return (false, "Invalid verification code");
        }

        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);

        // Remove verification claims
        await _userManager.RemoveClaimAsync(user, codeClaim);
        await _userManager.RemoveClaimAsync(user, expiryClaim);

        return (true, "Email verified successfully");
    }

    public async Task<(bool success, string message)> RequestVerificationCodeAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return (false, "User not found");
        }

        if (user.EmailConfirmed)
        {
            return (false, "Email is already verified");
        }

        var verificationCode = GenerateVerificationCode();
        await _emailService.SendVerificationEmailAsync(user, verificationCode);

        // Remove existing verification claims
        var claims = await _userManager.GetClaimsAsync(user);
        var codeClaim = claims.FirstOrDefault(c => c.Type == "VerificationCode");
        var expiryClaim = claims.FirstOrDefault(c => c.Type == "VerificationCodeExpiry");

        if (codeClaim != null)
            await _userManager.RemoveClaimAsync(user, codeClaim);
        if (expiryClaim != null)
            await _userManager.RemoveClaimAsync(user, expiryClaim);

        // Add new verification claims
        await _userManager.AddClaimAsync(user, new Claim("VerificationCode", verificationCode));
        await _userManager.AddClaimAsync(
            user,
            new Claim("VerificationCodeExpiry", DateTime.UtcNow.AddMinutes(10).ToString("O"))
        );

        return (true, "Verification code sent successfully");
    }

    public async Task<(bool success, string message)> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return (true, "If the email exists, a password reset link will be sent");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink =
            $"{_configuration["FrontendUrl"]}/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";

        await _emailService.SendPasswordResetEmailAsync(email, resetLink);
        return (true, "If the email exists, a password reset link will be sent");
    }

    public async Task<(bool success, string message)> ResetPasswordAsync(ResetPasswordDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return (false, "Invalid request");
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        if (!result.Succeeded)
        {
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return (true, "Password has been reset successfully");
    }

    public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"] ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"] ?? string.Empty),
            new Claim("UserId", user.Id),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
            new Claim("FullName", user.FullName ?? string.Empty),
        };

        // Add tenant claims if tenant exists
        if (user.TenantId.HasValue)
        {
            claims.Add(new Claim("TenantId", user.TenantId.Value.ToString()));
            claims.Add(new Claim("TenantIdentifier", user.TenantIdentifier ?? string.Empty));
        }

        claims.AddRange(roleClaims);

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]
                    ?? throw new InvalidOperationException("JWT Key not found")
            )
        );
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateVerificationCode()
    {
        return new Random().Next(100000, 999999).ToString();
    }
}
