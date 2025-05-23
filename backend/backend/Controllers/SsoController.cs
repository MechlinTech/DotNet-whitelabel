using System.IdentityModel.Tokens.Jwt;
using backend.DTOs.Auth;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SsoController : ControllerBase
{
    private readonly ISsoService _ssoService;
    private readonly ILogger<SsoController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITenantService _tenantService;

    public SsoController(
        ISsoService ssoService,
        ILogger<SsoController> logger,
        UserManager<ApplicationUser> userManager,
        ITenantService tenantService
    )
    {
        _ssoService = ssoService;
        _logger = logger;
        _userManager = userManager;
        _tenantService = tenantService;
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDTO model)
    {
        var (success, message, token, email) = await _ssoService.GoogleLoginAsync(model.IdToken);
        if (!success || string.IsNullOrEmpty(email))
        {
            return BadRequest(new { message });
        }

        var user = await _userManager.FindByEmailAsync(email);
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

    [HttpPost("microsoft")]
    public async Task<IActionResult> MicrosoftLogin([FromBody] MicrosoftLoginDTO model)
    {
        var (success, message, token, email) = await _ssoService.MicrosoftLoginAsync(model.IdToken);
        if (!success || string.IsNullOrEmpty(email))
        {
            return BadRequest(new { message });
        }

        var user = await _userManager.FindByEmailAsync(email);
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
}
