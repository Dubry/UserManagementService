using Serilog;
using System.Text.Json;
using UserManagementService.Exceptions;
using UserManagementService.Models;

namespace UserManagementService.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;
            var env = context.RequestServices.GetRequiredService<IHostEnvironment>();
            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            var clientName = context.Items["ClientName"]?.ToString() ?? "Unknown";
            var host = Environment.MachineName;
            var methodName = $"{context.Request.Method} {context.Request.Path}";

            int statusCode;
            string error;

            if (exception is AppException appEx)
            {
                statusCode = appEx.StatusCode;
                error = appEx.GetType().Name;
            }
            else
            {
                statusCode = StatusCodes.Status500InternalServerError;
                error = "InternalServerError";
            }

            Log.Error(exception,
                "Unhandled exception | ClientIP={ClientIP} Client={ClientName} Host={Host} Method={Method} TraceId={TraceId}",
                clientIp,
                clientName,
                host,
                methodName,
                traceId);

            var response = new ErrorResponse
            {
                StatusCode = statusCode,
                Error = error,
                Message = statusCode == 500
                    ? env.IsEnvironment("Test") || env.IsDevelopment()
                        ? exception.Message
                        : "An unexpected error occurred."
                    : exception.Message,
                TraceId = traceId
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
