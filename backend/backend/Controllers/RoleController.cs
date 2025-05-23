using backend.DTOs.Auth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Only admins can manage roles
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly ILogger<RoleController> _logger;

    public RoleController(IRoleService roleService, ILogger<RoleController> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    [HttpPost("init")]
    [AllowAnonymous]
    public async Task<IActionResult> InitializeUserRole()
    {
        var (success, message) = await _roleService.CreateRoleAsync("User");
        if (!success && message != "Role already exists")
        {
            return BadRequest(new { message });
        }

        return Ok(new { message = "User role initialized successfully" });
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return BadRequest(new { message = "Role name cannot be empty" });
        }

        _logger.LogInformation("Attempting to create role: {RoleName}", roleName);

        var (success, message) = await _roleService.CreateRoleAsync(roleName);
        if (!success)
        {
            _logger.LogWarning(
                "Failed to create role: {RoleName}. Error: {Message}",
                roleName,
                message
            );
            return BadRequest(new { message });
        }

        _logger.LogInformation("Successfully created role: {RoleName}", roleName);
        return Ok(new { message });
    }

    [HttpDelete("{roleName}")]
    public async Task<IActionResult> DeleteRole(string roleName)
    {
        var (success, message) = await _roleService.DeleteRoleAsync(roleName);
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message });
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRoleToUser([FromBody] RoleAssignmentDTO model)
    {
        var (success, message) = await _roleService.AssignRoleToUserAsync(
            model.UserId,
            model.RoleName
        );
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message });
    }

    [HttpPost("remove")]
    public async Task<IActionResult> RemoveRoleFromUser([FromBody] RoleAssignmentDTO model)
    {
        var (success, message) = await _roleService.RemoveRoleFromUserAsync(
            model.UserId,
            model.RoleName
        );
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var (success, message, roles) = await _roleService.GetAllRolesAsync();
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message, roles });
    }

    [HttpGet("by-role/{roleName}/users")]
    public async Task<IActionResult> GetUsersInRole(string roleName)
    {
        var (success, message, users) = await _roleService.GetUsersInRoleAsync(roleName);
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message, users });
    }

    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var (success, message, roles) = await _roleService.GetUserRolesAsync(userId);
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message, roles });
    }
}
