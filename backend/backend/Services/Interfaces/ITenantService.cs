using backend.DTOs.Tenant;
using backend.Models;

namespace backend.Services.Interfaces;

public interface ITenantService
{
    Task<(bool success, string message, TenantResponseDTO? tenant)> CreateTenantAsync(CreateTenantDTO dto);
    Task<(bool success, string message, TenantResponseDTO? tenant)> UpdateTenantAsync(Guid id, UpdateTenantDTO dto);
    Task<(bool success, string message)> DeleteTenantAsync(Guid id);
    Task<(bool success, string message, TenantResponseDTO? tenant)> GetTenantByIdAsync(Guid id);
    Task<(bool success, string message, List<TenantResponseDTO> tenants)> GetAllTenantsAsync();
    Task<(bool success, string message)> AssignUserToTenantAsync(AssignUserToTenantDTO dto);
    Task<(bool success, string message)> RemoveUserFromTenantAsync(string userId, Guid tenantId);
    Task<(bool success, string message, List<string> userIds)> GetUsersInTenantAsync(Guid tenantId);
    Task<(bool success, string message, List<TenantResponseDTO> tenants)> GetUserTenantsAsync(string userId);
}