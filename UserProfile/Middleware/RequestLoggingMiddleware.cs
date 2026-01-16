using System.Diagnostics;
using System.Security.Claims;
using UserProfile.Services.LoggerService;

public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ICustomLoggerService _logger;

    public RequestLoggingMiddleware(RequestDelegate next,ICustomLoggerService logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }

        catch (Exception ex)
        {
            // Log unhandled exception with context (NO body, NO secrets)
            await _logger.Critical(
                message: "Unhandled exception during HTTP request",
                userIdentifier: GetActorId(context),
                logEvent: "HTTP_UNHANDLED_EXCEPTION",
                statusCode: StatusCodes.Status500InternalServerError
            );

            throw; // let exception middleware handle response
        }
        finally
        {
            stopwatch.Stop();

            await _logger.Info(
                message: $"{context.Request.Method} {context.Request.Path}",
                logEvent: "HTTP_REQUEST_COMPLETED",
                statusCode: context.Response.StatusCode
            );
        }
    }

    private static string? GetActorId(HttpContext context)
    {
        // JWT / OIDC standard claim
        return context.User?.FindFirst("sub")?.Value
            ?? context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
