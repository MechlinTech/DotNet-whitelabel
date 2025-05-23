using backend.DTOs.Tenant;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Only admins can manage tenants
public class TenantController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly ILogger<TenantController> _logger;

    public TenantController(ITenantService tenantService, ILogger<TenantController> logger)
    {
        _tenantService = tenantService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantDTO dto)
    {
        var (success, message, tenant) = await _tenantService.CreateTenantAsync(dto);
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message, tenant });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTenant(Guid id, [FromBody] UpdateTenantDTO dto)
    {
        var (success, message, tenant) = await _tenantService.UpdateTenantAsync(id, dto);
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message, tenant });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTenant(Guid id)
    {
        var (success, message) = await _tenantService.DeleteTenantAsync(id);
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTenant(Guid id)
    {
        var (success, message, tenant) = await _tenantService.GetTenantByIdAsync(id);
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message, tenant });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTenants()
    {
        var (success, message, tenants) = await _tenantService.GetAllTenantsAsync();
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message, tenants });
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignUserToTenant([FromBody] AssignUserToTenantDTO dto)
    {
        var (success, message) = await _tenantService.AssignUserToTenantAsync(dto);
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message });
    }

    [HttpDelete("users/{userId}/tenants/{tenantId}")]
    public async Task<IActionResult> RemoveUserFromTenant(string userId, Guid tenantId)
    {
        var (success, message) = await _tenantService.RemoveUserFromTenantAsync(userId, tenantId);
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message });
    }

    [HttpGet("{tenantId}/users")]
    public async Task<IActionResult> GetUsersInTenant(Guid tenantId)
    {
        var (success, message, userIds) = await _tenantService.GetUsersInTenantAsync(tenantId);
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message, userIds });
    }

    [HttpGet("users/{userId}/tenants")]
    public async Task<IActionResult> GetUserTenants(string userId)
    {
        var (success, message, tenants) = await _tenantService.GetUserTenantsAsync(userId);
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message, tenants });
    }
}
