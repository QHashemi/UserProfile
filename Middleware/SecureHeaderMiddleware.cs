using System.Security.Claims;
using UserProfile.Utils;

namespace UserProfile.Middleware
{
    public class SecureHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICustomLogger _logger;

        public SecureHeaderMiddleware(RequestDelegate next, ICustomLogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Apply security headers
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["Referrer-Policy"] = "no-referrer";
            context.Response.Headers["Permissions-Policy"] = "geolocation=()";

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await _logger.Critical(
                    message: $"Unhandled exception: {ex.Message}",
                    logEvent: "HTTP_REQUEST_EXCEPTION",
                    statusCode: 500,
                    userIdentifier: GetActorId(context) // <-- now defined
                );

                throw;
            }
            finally
            {
                await _logger.Info(
                    message: $"{context.Request.Method} {context.Request.Path}",
                    logEvent: "HTTP_REQUEST_COMPLETED",
                    statusCode: context.Response.StatusCode,
                    userIdentifier: GetActorId(context)
                );
            }
        }

        // Helper method to get the user identifier from JWT or ClaimsPrincipal
        private static string? GetActorId(HttpContext context)
        {
            return context.User?.FindFirst("sub")?.Value
                   ?? context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
