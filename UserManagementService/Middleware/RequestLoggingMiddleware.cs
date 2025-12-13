using Serilog;

namespace UserManagementService.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            var clientName = context.Items["ClientName"]?.ToString() ?? "Unknown";
            var host = Environment.MachineName;
            var methodName = $"{context.Request.Method} {context.Request.Path}";

            context.Request.EnableBuffering();

            string requestBody = "";
            if (context.Request.ContentLength > 0)
            {
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            try
            {
                await _next(context);

                Log.Information(
                    "ClientIP={ClientIP}, Client={ClientName}, Host={Host}, Method={Method}, Params={Params}, Message=Success",
                    clientIp,
                    clientName,
                    host,
                    methodName,
                    requestBody);
            }
            catch (Exception ex)
            {
                Log.Error(
                    ex,
                    "ClientIP={ClientIP}, Client={ClientName}, Host={Host}, Method={Method}, Params={Params}, Message=Error",
                    clientIp,
                    clientName,
                    host,
                    methodName,
                    requestBody);

                throw;
            }
        }
    }
}
