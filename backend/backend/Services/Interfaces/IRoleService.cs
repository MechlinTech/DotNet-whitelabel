using backend.Models;

namespace backend.Services.Interfaces;

public interface IRoleService
{
    Task<(bool success, string message)> CreateRoleAsync(string roleName);
    Task<(bool success, string message)> DeleteRoleAsync(string roleName);
    Task<(bool success, string message)> AssignRoleToUserAsync(string userId, string roleName);
    Task<(bool success, string message)> RemoveRoleFromUserAsync(string userId, string roleName);
    Task<(bool success, string message, List<string> roles)> GetUserRolesAsync(string userId);
    Task<(bool success, string message, List<string> roles)> GetAllRolesAsync();
    Task<(bool success, string message, List<ApplicationUser> users)> GetUsersInRoleAsync(
        string roleName
    );
}
