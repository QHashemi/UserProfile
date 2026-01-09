using UserProfile.Utils;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ICustomLogger _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ICustomLogger logger)
    {
        _next = next;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _logger.Info($"Incoming Request: {context.Request.Method} {context.Request.Path}");
            await _next(context);
            await _logger.Info($"Response Status Code: {context.Response.StatusCode}");
        }
        catch (Exception ex)
        {
            await _logger.Error("Unhandled exception in middleware", ex);
            throw;
        }
    }
}
