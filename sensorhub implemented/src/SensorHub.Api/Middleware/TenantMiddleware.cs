using SensorHub.Application.Interfaces;

namespace SensorHub.Api.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ICurrentTenantAccessor tenantAccessor)
    {
        if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var header)
            && Guid.TryParse(header, out var tenantId))
        {
            tenantAccessor.TenantId = tenantId;
        }

        await _next(context);
    }
}
