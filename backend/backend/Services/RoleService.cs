using backend.Models;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace backend.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ILogger<RoleService> logger
    )
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<(bool success, string message)> CreateRoleAsync(string roleName)
    {
        try
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return (false, "Role already exists");
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                return (true, "Role created successfully");
            }

            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role {RoleName}", roleName);
            return (false, "An error occurred while creating the role");
        }
    }

    public async Task<(bool success, string message)> DeleteRoleAsync(string roleName)
    {
        try
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return (false, "Role not found");
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return (true, "Role deleted successfully");
            }

            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleName}", roleName);
            return (false, "An error occurred while deleting the role");
        }
    }

    public async Task<(bool success, string message)> AssignRoleToUserAsync(
        string userId,
        string roleName
    )
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found");
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return (false, "Role not found");
            }

            // Get user's current roles
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remove all existing roles
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    return (
                        false,
                        string.Join(", ", removeResult.Errors.Select(e => e.Description))
                    );
                }
            }

            // Assign the new role
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return (true, "Role assigned successfully");
            }

            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error assigning role {RoleName} to user {UserId}",
                roleName,
                userId
            );
            return (false, "An error occurred while assigning the role");
        }
    }

    public async Task<(bool success, string message)> RemoveRoleFromUserAsync(
        string userId,
        string roleName
    )
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found");
            }

            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                return (false, "User does not have this role");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return (true, "Role removed successfully");
            }

            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error removing role {RoleName} from user {UserId}",
                roleName,
                userId
            );
            return (false, "An error occurred while removing the role");
        }
    }

    public async Task<(bool success, string message, List<string> roles)> GetUserRolesAsync(
        string userId
    )
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found", new List<string>());
            }

            var roles = await _userManager.GetRolesAsync(user);
            return (true, "Roles retrieved successfully", roles.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for user {UserId}", userId);
            return (false, "An error occurred while retrieving roles", new List<string>());
        }
    }

    public async Task<(bool success, string message, List<string> roles)> GetAllRolesAsync()
    {
        try
        {
            var roles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
            return (true, "Roles retrieved successfully", roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all roles");
            return (false, "An error occurred while retrieving roles", new List<string>());
        }
    }

    public async Task<(
        bool success,
        string message,
        List<ApplicationUser> users
    )> GetUsersInRoleAsync(string roleName)
    {
        try
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return (false, "Role not found", new List<ApplicationUser>());
            }

            var users = await _userManager.GetUsersInRoleAsync(roleName);
            return (true, "Users retrieved successfully", users.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users in role {RoleName}", roleName);
            return (false, "An error occurred while retrieving users", new List<ApplicationUser>());
        }
    }
}
