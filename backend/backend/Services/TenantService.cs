using System.Data.SqlClient;
using AutoMapper;
using backend.DBContext;
using backend.DTOs.Tenant;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace backend.Services;

public class TenantService : ITenantService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<TenantService> _logger;
    private readonly IConfiguration _configuration;

    public TenantService(
        ApplicationDbContext context,
        IMapper mapper,
        ILogger<TenantService> logger,
        IConfiguration configuration
    )
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _configuration = configuration;

        // Configure AutoMapper
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Tenant, TenantResponseDTO>();
            cfg.CreateMap<CreateTenantDTO, Tenant>();
            cfg.CreateMap<UpdateTenantDTO, Tenant>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        });
        _mapper = config.CreateMapper();
    }

    public async Task<(bool success, string message, TenantResponseDTO? tenant)> CreateTenantAsync(
        CreateTenantDTO dto
    )
    {
        try
        {
            if (await _context.Tenants.AnyAsync(t => t.Identifier == dto.Identifier))
            {
                return (false, "Tenant with this identifier already exists", null);
            }

            // Generate database name from domain and identifier
            var databaseName = GenerateDatabaseName(dto.Domain, dto.Identifier);

            // Create new database for tenant
            var connectionString = await CreateTenantDatabaseAsync(databaseName);
            if (string.IsNullOrEmpty(connectionString))
            {
                return (false, "Failed to create tenant database", null);
            }

            var tenant = new Tenant
            {
                Name = dto.Name,
                Identifier = dto.Identifier,
                Description = dto.Description,
                DatabaseName = databaseName,
                ConnectionString = connectionString,
                LogoUrl = dto.LogoUrl,
                Theme = dto.Theme,
                Domain = dto.Domain,
                SubscriptionPlan = dto.SubscriptionPlan,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            // Initialize tenant database with required tables
            await InitializeTenantDatabaseAsync(connectionString);

            return (true, "Tenant created successfully", _mapper.Map<TenantResponseDTO>(tenant));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tenant: {Message}", ex.Message);
            return (false, $"Error creating tenant: {ex.Message}", null);
        }
    }

    private string GenerateDatabaseName(string? domain, string identifier)
    {
        // Clean identifier (remove special characters and spaces)
        var cleanIdentifier = identifier
            .ToLower()
            .Replace(" ", "")
            .Replace(".", "")
            .Replace("-", "")
            .Replace("_", "");

        // Combine domain and identifier with timestamp to ensure uniqueness
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        return $"{cleanIdentifier}_{timestamp}_db";
    }

    private async Task<string> CreateTenantDatabaseAsync(string databaseName)
    {
        try
        {
            var masterConnectionString = _configuration.GetConnectionString("MasterConnection");
            if (string.IsNullOrEmpty(masterConnectionString))
            {
                _logger.LogError("Master connection string is missing");
                return string.Empty;
            }

            using var connection = new SqlConnection(masterConnectionString);
            await connection.OpenAsync();

            // Check if database already exists
            using var checkCommand = new SqlCommand(
                $"SELECT COUNT(*) FROM sys.databases WHERE name = '{databaseName}'",
                connection
            );
            var result = await checkCommand.ExecuteScalarAsync();
            var exists = result != null && Convert.ToInt32(result) > 0;
            if (exists)
            {
                _logger.LogWarning("Database {DatabaseName} already exists", databaseName);
                return string.Empty;
            }

            // Create database
            using var command = new SqlCommand($"CREATE DATABASE [{databaseName}]", connection);
            await command.ExecuteNonQueryAsync();

            // Get the new database connection string
            var defaultConnection = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(defaultConnection))
            {
                _logger.LogError("Default connection string is missing");
                return string.Empty;
            }

            // Extract server and other parameters from default connection
            var builder = new SqlConnectionStringBuilder(defaultConnection);
            builder.InitialCatalog = databaseName; // Set the database name
            return builder.ConnectionString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tenant database: {Message}", ex.Message);
            return string.Empty;
        }
    }

    private async Task InitializeTenantDatabaseAsync(string connectionString)
    {
        try
        {
            var options = new DbContextOptionsBuilder<TenantDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            using var context = new TenantDbContext(options);

            // Ensure database exists
            await context.Database.EnsureCreatedAsync();

            // Apply any pending migrations
            if (context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing tenant database: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<(bool success, string message, TenantResponseDTO? tenant)> UpdateTenantAsync(
        Guid id,
        UpdateTenantDTO dto
    )
    {
        try
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
            {
                return (false, "Tenant not found", null);
            }

            if (dto.Name != null)
                tenant.Name = dto.Name;
            if (dto.Description != null)
                tenant.Description = dto.Description;
            if (dto.IsActive.HasValue)
                tenant.IsActive = dto.IsActive.Value;
            if (dto.LogoUrl != null)
                tenant.LogoUrl = dto.LogoUrl;
            if (dto.Theme != null)
                tenant.Theme = dto.Theme;
            if (dto.Domain != null)
                tenant.Domain = dto.Domain;
            if (dto.SubscriptionPlan != null)
                tenant.SubscriptionPlan = dto.SubscriptionPlan;
            if (dto.SubscriptionExpiry.HasValue)
                tenant.SubscriptionExpiry = dto.SubscriptionExpiry.Value;

            tenant.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return (true, "Tenant updated successfully", _mapper.Map<TenantResponseDTO>(tenant));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tenant");
            return (false, "Error updating tenant", null);
        }
    }

    public async Task<(bool success, string message)> DeleteTenantAsync(Guid id)
    {
        try
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
            {
                return (false, "Tenant not found");
            }

            // Drop tenant database
            var masterConnectionString = _configuration.GetConnectionString("MasterConnection");
            using var connection = new SqlConnection(masterConnectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                $"DROP DATABASE [{tenant.DatabaseName}]",
                connection
            );
            await command.ExecuteNonQueryAsync();

            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();

            return (true, "Tenant deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tenant");
            return (false, "Error deleting tenant");
        }
    }

    public async Task<(bool success, string message, TenantResponseDTO? tenant)> GetTenantByIdAsync(
        Guid id
    )
    {
        try
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
            {
                return (false, "Tenant not found", null);
            }

            return (true, "Tenant retrieved successfully", _mapper.Map<TenantResponseDTO>(tenant));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tenant");
            return (false, "Error retrieving tenant", null);
        }
    }

    public async Task<(
        bool success,
        string message,
        List<TenantResponseDTO> tenants
    )> GetAllTenantsAsync()
    {
        try
        {
            var tenants = await _context.Tenants.ToListAsync();
            return (
                true,
                "Tenants retrieved successfully",
                _mapper.Map<List<TenantResponseDTO>>(tenants)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tenants");
            return (false, "Error retrieving tenants", new List<TenantResponseDTO>());
        }
    }

    public async Task<(bool success, string message)> AssignUserToTenantAsync(
        AssignUserToTenantDTO dto
    )
    {
        try
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
            {
                return (false, "User not found");
            }

            var tenant = await _context.Tenants.FindAsync(dto.TenantId);
            if (tenant == null)
            {
                return (false, "Tenant not found");
            }

            user.TenantId = tenant.Id;
            user.TenantIdentifier = tenant.Identifier;

            await _context.SaveChangesAsync();

            return (true, "User assigned to tenant successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning user to tenant");
            return (false, "Error assigning user to tenant");
        }
    }

    public async Task<(bool success, string message)> RemoveUserFromTenantAsync(
        string userId,
        Guid tenantId
    )
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return (false, "User not found");
            }

            if (user.TenantId != tenantId)
            {
                return (false, "User is not assigned to this tenant");
            }

            user.TenantId = null;
            user.TenantIdentifier = null;

            await _context.SaveChangesAsync();

            return (true, "User removed from tenant successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user from tenant");
            return (false, "Error removing user from tenant");
        }
    }

    public async Task<(bool success, string message, List<string> userIds)> GetUsersInTenantAsync(
        Guid tenantId
    )
    {
        try
        {
            var userIds = await _context
                .Users.Where(u => u.TenantId == tenantId)
                .Select(u => u.Id)
                .ToListAsync();

            return (true, "Users retrieved successfully", userIds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users in tenant");
            return (false, "Error retrieving users in tenant", new List<string>());
        }
    }

    public async Task<(
        bool success,
        string message,
        List<TenantResponseDTO> tenants
    )> GetUserTenantsAsync(string userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return (false, "User not found", new List<TenantResponseDTO>());
            }

            var tenant = await _context.Tenants.FindAsync(user.TenantId);
            if (tenant == null)
            {
                return (true, "User has no assigned tenant", new List<TenantResponseDTO>());
            }

            return (
                true,
                "User tenant retrieved successfully",
                new List<TenantResponseDTO> { _mapper.Map<TenantResponseDTO>(tenant) }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user tenant");
            return (false, "Error retrieving user tenant", new List<TenantResponseDTO>());
        }
    }
}
