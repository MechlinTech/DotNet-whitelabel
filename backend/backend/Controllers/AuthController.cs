using backend.DBContext;
using backend.DTOs.Auth;
using backend.DTOs.Tenant;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITenantService _tenantService;
    private readonly ILogger<AuthController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public AuthController(
        IAuthService authService,
        ITenantService tenantService,
        ILogger<AuthController> logger,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context
    )
    {
        _authService = authService;
        _tenantService = tenantService;
        _logger = logger;
        _userManager = userManager;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO model)
    {
        var (success, message, token) = await _authService.RegisterAsync(model);
        if (!success)
        {
            return BadRequest(new { message });
        }

        // If tenant identifier is provided, assign user to tenant
        if (!string.IsNullOrEmpty(model.TenantIdentifier))
        {
            var existingTenant = await _context.Tenants.FirstOrDefaultAsync(t =>
                t.Identifier == model.TenantIdentifier
            );
            if (existingTenant != null)
            {
                var newUser = await _userManager.FindByEmailAsync(model.Email);
                if (newUser != null)
                {
                    await _tenantService.AssignUserToTenantAsync(
                        new AssignUserToTenantDTO
                        {
                            UserId = newUser.Id,
                            TenantId = existingTenant.Id,
                        }
                    );
                }
            }
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        var tenant =
            user?.TenantId.HasValue == true
                ? await _tenantService.GetTenantByIdAsync(user.TenantId.Value)
                : (false, "No tenant", null);

        return Ok(
            new
            {
                message,
                token,
                tenant = tenant.success
                    ? new
                    {
                        id = tenant.tenant?.Id,
                        identifier = tenant.tenant?.Identifier,
                        name = tenant.tenant?.Name,
                    }
                    : null,
            }
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO model)
    {
        var (success, message, token) = await _authService.LoginAsync(model);
        if (!success)
        {
            return Unauthorized(new { message });
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        var tenant =
            user?.TenantId.HasValue == true
                ? await _tenantService.GetTenantByIdAsync(user.TenantId.Value)
                : (false, "No tenant", null);

        return Ok(
            new
            {
                message,
                token,
                tenant = tenant.success
                    ? new
                    {
                        id = tenant.tenant?.Id,
                        identifier = tenant.tenant?.Identifier,
                        name = tenant.tenant?.Name,
                    }
                    : null,
            }
        );
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] EmailVerificationDTO model)
    {
        var (success, message) = await _authService.VerifyEmailAsync(model);
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message });
    }

    [HttpPost("request-verification-code")]
    public async Task<IActionResult> RequestVerificationCode([FromBody] ForgotPasswordDTO model)
    {
        var (success, message) = await _authService.RequestVerificationCodeAsync(model.Email);
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
    {
        var (success, message) = await _authService.ForgotPasswordAsync(model.Email);
        return Ok(new { message });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
    {
        var (success, message) = await _authService.ResetPasswordAsync(model);
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message });
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var tenantInfo = user.TenantId.HasValue
            ? await _tenantService.GetTenantByIdAsync(user.TenantId.Value)
            : (false, "No tenant assigned", null);

        return Ok(
            new
            {
                email = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName,
                fullName = user.FullName,
                emailConfirmed = user.EmailConfirmed,
                roles = roles,
                tenant = tenantInfo.success
                    ? new
                    {
                        id = tenantInfo.tenant?.Id,
                        name = tenantInfo.tenant?.Name,
                        identifier = tenantInfo.tenant?.Identifier,
                    }
                    : null,
            }
        );
    }

    [Authorize]
    [HttpGet("role")]
    public async Task<IActionResult> GetUserRole()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";

        return Ok(new { role });
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDtos = users.Select(u => new
        {
            id = u.Id,
            email = u.Email,
            name = u.FullName,
            tenantId = u.TenantId,
            roles = _userManager.GetRolesAsync(u).Result,
        });
        return Ok(new { users = userDtos });
    }
}
