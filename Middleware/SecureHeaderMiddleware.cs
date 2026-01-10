
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

                // Optional: log successful request
                await _logger.Info(
                    message: $"{context.Request.Method} {context.Request.Path}",
                    logEvent: "HTTP_REQUEST_COMPLETED",
                    statusCode: context.Response.StatusCode
                );
            }
            catch (Exception ex)
            {
                // Log exception
                await _logger.Critical(
                    message: $"Unhandled exception: {ex.Message}",
                    logEvent: "HTTP_REQUEST_EXCEPTION",
                    statusCode: 500
                );

                throw;
            }
        }
    }
}
