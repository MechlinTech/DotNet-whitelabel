using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using backend.Models;
using backend.Services.Interfaces;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace backend.Services;

public class SsoService : ISsoService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SsoService> _logger;
    private readonly IAuthService _authService;

    public SsoService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger<SsoService> logger,
        IAuthService authService
    )
    {
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
        _authService = authService;
    }

    public async Task<(bool success, string message, string? token, string? email)> GoogleLoginAsync(
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
                        null,
                        null
                    );
                }

                await _userManager.AddToRoleAsync(user, "User");
            }
            else if (user.PasswordHash != null)
            {
                return (false, "This email is already registered. Please use regular login.", null, null);
            }

            var token = await _authService.GenerateJwtTokenAsync(user);
            return (true, "Google login successful", token, payload.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Google login failed");
            return (false, "Google authentication failed", null, null);
        }
    }

    public async Task<(bool success, string message, string? token, string? email)> MicrosoftLoginAsync(
        string idToken
    )
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(idToken) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return (false, "Invalid token format", null, null);
            }

            var email = jsonToken.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            var name = jsonToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var givenName = jsonToken.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value;
            var familyName = jsonToken.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return (false, "Email not found in token", null, null);
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                // Register new user
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName =
                        givenName ?? name?.Split(' ').FirstOrDefault() ?? email.Split('@').First(),
                    LastName =
                        familyName
                        ?? (
                            name?.Split(' ').Length > 1
                                ? string.Join(" ", name.Split(' ').Skip(1))
                                : "User"
                        ),
                    FullName = name ?? email,
                    EmailConfirmed = true,
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return (
                        false,
                        string.Join(", ", result.Errors.Select(e => e.Description)),
                        null,
                        null
                    );
                }

                await _userManager.AddToRoleAsync(user, "User");
            }
            else if (user.PasswordHash != null)
            {
                return (false, "This email is already registered. Please use regular login.", null, null);
            }

            var token = await _authService.GenerateJwtTokenAsync(user);
            return (true, "Microsoft login successful", token, email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Microsoft login failed");
            return (false, "Microsoft authentication failed", null, null);
        }
    }
}
