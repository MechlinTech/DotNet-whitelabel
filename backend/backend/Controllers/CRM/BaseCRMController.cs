using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace backend.Controllers.CRM
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseCRMController : ControllerBase
    {
        protected readonly ILogger _logger;

        protected BaseCRMController(ILogger logger)
        {
            _logger = logger;
        }

        protected Guid GetTenantId()
        {
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;
            if (string.IsNullOrEmpty(tenantIdClaim))
            {
                _logger.LogWarning("TenantId claim not found in user claims");
                throw new UnauthorizedAccessException(
                    "User is not associated with any tenant. Please contact your administrator."
                );
            }

            if (!Guid.TryParse(tenantIdClaim, out Guid tenantId))
            {
                _logger.LogError("Invalid tenant ID format: {TenantId}", tenantIdClaim);
                throw new InvalidOperationException("Invalid tenant ID format");
            }

            return tenantId;
        }

        protected bool HasRole(string role)
        {
            return User.IsInRole(role);
        }

        protected string GetTenantIdentifier()
        {
            var tenantIdentifier = User.FindFirst("TenantIdentifier")?.Value;
            if (string.IsNullOrEmpty(tenantIdentifier))
            {
                _logger.LogWarning("TenantIdentifier claim not found in user claims");
                throw new UnauthorizedAccessException("Tenant identifier not found in claims");
            }
            return tenantIdentifier;
        }

        protected ActionResult<T> HandleException<T>(Exception ex)
        {
            _logger.LogError(ex, "Error occurred in CRM controller: {Message}", ex.Message);

            return ex switch
            {
                UnauthorizedAccessException => Unauthorized(new { message = ex.Message }),
                KeyNotFoundException => NotFound(new { message = "Resource not found" }),
                InvalidOperationException => BadRequest(new { message = ex.Message }),
                DbUpdateException => StatusCode(
                    500,
                    new { message = "Database operation failed. Please try again." }
                ),
                _ => StatusCode(
                    500,
                    new { message = $"An unexpected error occurred: {ex.Message}" }
                ),
            };
        }

        protected IActionResult HandleException(Exception ex)
        {
            _logger.LogError(ex, "Error occurred in CRM controller: {Message}", ex.Message);

            return ex switch
            {
                UnauthorizedAccessException => Unauthorized(new { message = ex.Message }),
                KeyNotFoundException => NotFound(new { message = "Resource not found" }),
                InvalidOperationException => BadRequest(new { message = ex.Message }),
                DbUpdateException => StatusCode(
                    500,
                    new { message = "Database operation failed. Please try again." }
                ),
                _ => StatusCode(
                    500,
                    new { message = $"An unexpected error occurred: {ex.Message}" }
                ),
            };
        }
    }
}
