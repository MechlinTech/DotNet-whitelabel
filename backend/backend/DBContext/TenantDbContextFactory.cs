using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace backend.DBContext;

public class TenantDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TenantDbContextFactory> _logger;

    public TenantDbContextFactory(
        IConfiguration configuration,
        ILogger<TenantDbContextFactory> logger
    )
    {
        _configuration = configuration;
        _logger = logger;
    }

    public ApplicationDbContext CreateMainDbContext()
    {
        var connectionString =
            _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Default connection string not found");

        return new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connectionString)
                .Options
        );
    }

    public TenantDbContext CreateTenantDbContext(string tenantIdentifier)
    {
        if (string.IsNullOrEmpty(tenantIdentifier))
        {
            throw new ArgumentException("Tenant identifier is required", nameof(tenantIdentifier));
        }

        // Get the tenant's connection string from the default database
        using var mainContext = CreateMainDbContext();
        var tenant = mainContext.Tenants.FirstOrDefault(t => t.Identifier == tenantIdentifier);

        if (tenant == null)
        {
            throw new InvalidOperationException(
                $"Tenant with identifier {tenantIdentifier} not found"
            );
        }

        if (string.IsNullOrEmpty(tenant.ConnectionString))
        {
            throw new InvalidOperationException(
                $"Connection string not found for tenant {tenantIdentifier}"
            );
        }

        // Create a new context with the tenant's connection string
        return new TenantDbContext(
            new DbContextOptionsBuilder<TenantDbContext>()
                .UseSqlServer(tenant.ConnectionString)
                .Options
        );
    }

    public async Task<TenantDbContext> CreateTenantDbContextAsync(string tenantIdentifier)
    {
        if (string.IsNullOrEmpty(tenantIdentifier))
        {
            throw new ArgumentException("Tenant identifier is required", nameof(tenantIdentifier));
        }

        // Get the tenant's connection string from the default database
        using var mainContext = CreateMainDbContext();
        var tenant = await mainContext.Tenants.FirstOrDefaultAsync(t =>
            t.Identifier == tenantIdentifier
        );

        if (tenant == null)
        {
            throw new InvalidOperationException(
                $"Tenant with identifier {tenantIdentifier} not found"
            );
        }

        if (string.IsNullOrEmpty(tenant.ConnectionString))
        {
            throw new InvalidOperationException(
                $"Connection string not found for tenant {tenantIdentifier}"
            );
        }

        // Create a new context with the tenant's connection string
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseSqlServer(tenant.ConnectionString)
            .Options;

        var context = new TenantDbContext(options);

        try
        {
            // Ensure database exists and is up to date
            await context.Database.EnsureCreatedAsync();

            // Apply any pending migrations
            if (context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
            }

            _logger.LogInformation($"Tenant database for {tenantIdentifier} is ready");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error ensuring tenant database for {tenantIdentifier} is ready");
            throw;
        }

        return context;
    }

    public async Task EnsureTenantDatabaseExistsAsync(
        string tenantIdentifier,
        string connectionString
    )
    {
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        using var context = new TenantDbContext(options);

        try
        {
            // Create database if it doesn't exist
            await context.Database.EnsureCreatedAsync();

            // Apply any pending migrations
            if (context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
            }

            _logger.LogInformation($"Tenant database for {tenantIdentifier} created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating tenant database for {tenantIdentifier}");
            throw;
        }
    }

    public TenantDbContext CreateDbContext(string[] args)
    {
        // Get the connection string from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new TenantDbContext(optionsBuilder.Options);
    }
}
