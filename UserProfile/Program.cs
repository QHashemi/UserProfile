using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http.HttpResults;
using UserProfile.Config;
using UserProfile.Middleware;
using UserProfile.Startup;


// SERVICES =========================================================================================>
var builder = WebApplication.CreateBuilder(args);

// Inject every Dependencies
builder.AddDependencies();

// MIDLLEWARES ===========================================================================================>
var app = builder.Build();

// Getting the datbase ready
app.MigrateDatabase();

// Use open to send request easly
app.UseOpenApi();

// Register your custom IP blocking middleware
app.UseMiddleware<BlockSuspiciousIpsMiddleware>();

// Add Rate Limit Middleware
app.UseIpRateLimiting();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // only in production
}

// 1️⃣ Exception handling must be first
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 2️⃣ Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// 30 Apply cors
app.ApplyCorsConfig();

// Map all health check
app.MapAllHealthChecks();

// 3️⃣ Security headers
app.UseMiddleware<SecureHeaderMiddleware>();

// 4️⃣ Log requests (metadata only)
app.UseMiddleware<RequestLoggingMiddleware>();

// 5️⃣ Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 6️⃣ Map controllers
app.MapControllers();

// ENDPOINTS
app.MapGet("/tester", () =>
{
    return Results.Ok("HELLOW WORLD.");
});

app.Run();
