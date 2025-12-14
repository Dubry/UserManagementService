using Microsoft.EntityFrameworkCore;
using Serilog;
using UserManagementService.Data;

namespace UserManagementService.Middleware
{
    public class ApiKeyMiddleware
    {
        private const string ApiKeyHeaderName = "X-API-KEY";
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiKeyMiddleware> _logger;

        public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext db)
        {
            if (context.Request.Path.StartsWithSegments("/api/users"))
            {
                if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyValues))
                {
                    _logger.LogWarning("API Key missing from request");

                    await WriteUnauthorizedAsync(context, "API Key missing");
                    return;
                }

                var apiKey = apiKeyValues.ToString();
                var client = await db.ApiClients
                    .FirstOrDefaultAsync(c => c.ApiKey == apiKey &&
                        c.IsActive);

                if (client == null)
                {
                    _logger.LogWarning("Invalid API key attempted: {ApiKey}", apiKey.ToString());

                    await WriteUnauthorizedAsync(context, "Invalid API Key");
                    return;
                }

                context.Items["ClientId"] = client.Id;
                context.Items["ClientName"] = client.ClientName;
            }

            await _next(context);
        }

        private static async Task WriteUnauthorizedAsync(HttpContext context, string message)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                statusCode = 401,
                error = "Unauthorized",
                message
            });
        }
    }
}
