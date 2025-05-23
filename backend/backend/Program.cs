using System.Security.Claims;
using System.Text;
using AutoMapper;
using backend.DBContext;
using backend.Middleware;
using backend.Models;
using backend.Services;
using backend.Services.CRM;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Load Configuration
builder
    .Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register TenantDbContext factory as singleton
builder.Services.AddSingleton<TenantDbContextFactory>();

// Register TenantDbContext as scoped service
builder.Services.AddScoped<TenantDbContext>(sp =>
{
    var factory = sp.GetRequiredService<TenantDbContextFactory>();
    var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    var tenantIdentifier = httpContext?.Items["TenantIdentifier"] as string;

    if (string.IsNullOrEmpty(tenantIdentifier))
    {
        throw new InvalidOperationException("Tenant identifier not found in context");
    }

    return factory.CreateTenantDbContext(tenantIdentifier);
});

// Identity Configuration with custom password requirements
builder
    .Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Dependency Injection
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISsoService, SsoService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ITenantService, TenantService>();

// Register CRM services
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IDealService, DealService>();

// Add AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
    cfg.AddProfile<CRMMappingProfile>();
});

// CORS Configuration with fallback
var allowedOriginsRaw =
    builder.Configuration.GetValue<string>("AllowedOrigins") ?? "http://localhost:4200";
var allowedOrigins = allowedOriginsRaw.Split(',');

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
    throw new InvalidOperationException("JWT Key is missing in configuration.");

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = key,
            RoleClaimType = ClaimTypes.Role,
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["ACCESS_TOKEN"];
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Append("Token-Expired", "true");
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(
                    new { message = "Invalid token" }
                );
                return context.Response.WriteAsync(result);
            },
        };
    });

// Health Checks
builder.Services.AddHealthChecks();

// Distributed Memory Cache and Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Cookie Policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.SameAsRequest;
});

// Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT token in the format: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement { { securityScheme, new string[] { } } }
    );
});

// Dynamic port configuration
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

var app = builder.Build();

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var tenantFactory = scope.ServiceProvider.GetRequiredService<TenantDbContextFactory>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Apply migrations to main database
        dbContext.Database.Migrate();

        // Create initial roles if they don't exist
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Create admin user if it doesn't exist
        var adminEmail = "aayushffc@gmail.com"; // Your email
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser != null)
        {
            // First remove User role if it exists
            if (await userManager.IsInRoleAsync(adminUser, "User"))
            {
                await userManager.RemoveFromRoleAsync(adminUser, "User");
            }

            // Then assign Admin role if not already assigned
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Create default admin user if it doesn't exist
        var defaultAdminEmail = "admin@gmail.com";
        var defaultAdminUser = await userManager.FindByEmailAsync(defaultAdminEmail);
        if (defaultAdminUser == null)
        {
            defaultAdminUser = new ApplicationUser
            {
                UserName = defaultAdminEmail,
                Email = defaultAdminEmail,
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(defaultAdminUser, "Pass@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(defaultAdminUser, "Admin");
                logger.LogInformation("Default admin user created successfully");
            }
            else
            {
                logger.LogError(
                    "Failed to create default admin user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );
            }
        }

        // Apply migrations to all tenant databases
        var tenants = await dbContext.Tenants.ToListAsync();
        foreach (var tenant in tenants)
        {
            try
            {
                logger.LogInformation($"Applying migrations for tenant: {tenant.Identifier}");

                // Create options for TenantDbContext
                var options = new DbContextOptionsBuilder<TenantDbContext>()
                    .UseSqlServer(tenant.ConnectionString)
                    .Options;

                // Create context and apply migrations
                using var tenantContext = new TenantDbContext(options);
                if (tenantContext.Database.GetPendingMigrations().Any())
                {
                    await tenantContext.Database.MigrateAsync();
                    logger.LogInformation(
                        $"Successfully applied migrations for tenant: {tenant.Identifier}"
                    );
                }
                else
                {
                    logger.LogInformation($"No pending migrations for tenant: {tenant.Identifier}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to apply migrations for tenant {tenant.Identifier}");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to apply database migrations or create initial roles");
        throw;
    }
}

// Middleware order
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHealthChecks("/health");
app.UseCookiePolicy();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Add tenant middleware after authentication but before endpoints
app.UseMiddleware<TenantMiddleware>();

app.MapControllers();

app.Run();
