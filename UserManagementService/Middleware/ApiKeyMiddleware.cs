using Microsoft.EntityFrameworkCore;
using UserManagementService.Data;

namespace UserManagementService.Middleware
{
    public class ApiKeyMiddleware
    {
        private const string ApiKeyHeaderName = "X-API-KEY";
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext db)
        {
            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API Key missing");
                return;
            }

            var client = await db.ApiClients
                .FirstOrDefaultAsync(c => c.ApiKey == apiKey);

            if (client == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            context.Items["ClientName"] = client.ClientName;

            await _next(context);
        }
    }
}
