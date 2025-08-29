using System.Security.Claims;

namespace FormsManagementApi.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Extract tenant information from JWT token
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = context.User.FindFirst("TenantId");
            var roleClaim = context.User.FindFirst(ClaimTypes.Role);

            if (tenantIdClaim != null && int.TryParse(tenantIdClaim.Value, out int tenantId))
            {
                context.Items["TenantId"] = tenantId;
            }

            if (roleClaim != null)
            {
                context.Items["UserRole"] = roleClaim.Value;
            }

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                context.Items["UserId"] = userId;
            }
        }

        await _next(context);
    }
}

public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantMiddleware>();
    }
}

public static class HttpContextExtensions
{
    public static int? GetTenantId(this HttpContext context)
    {
        return context.Items.TryGetValue("TenantId", out var tenantId) ? (int?)tenantId : null;
    }

    public static string? GetUserRole(this HttpContext context)
    {
        return context.Items.TryGetValue("UserRole", out var role) ? role?.ToString() : null;
    }

    public static int? GetUserId(this HttpContext context)
    {
        return context.Items.TryGetValue("UserId", out var userId) ? (int?)userId : null;
    }

    public static bool IsSuperAdmin(this HttpContext context)
    {
        return context.GetUserRole() == "SuperAdmin";
    }

    public static bool IsTenantAdmin(this HttpContext context)
    {
        return context.GetUserRole() == "TenantAdmin";
    }
}