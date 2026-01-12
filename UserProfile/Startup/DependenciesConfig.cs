using UserProfile.Config;
using UserProfile.Services.AuthService;
using UserProfile.Utils.Interfaces;

namespace UserProfile.Startup
{
    public static class DependenciesConfig
    {
        public static void AddDependencies(this WebApplicationBuilder builder)
        {

            // Register HttpContext accessor
            builder.Services.AddHttpContextAccessor();

            // Register Controllers
            builder.Services.AddControllers();

            // Register RateLimit for controll Multiple Request
            builder.Services.EndPointRateLimitConfig();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApiService();

            // Add Cors Service
            builder.Services.AddCorsServices();

            // Health check service
            builder.Services.AddAllHealthChecks();

            // Sql Server connection 
            builder.Services.AddAppDbContextConfig(builder.Configuration);

            // Secure end point configuration with jwt
            builder.Services.AddAuthenticationJwt(builder.Configuration);

            // Register Controllers so that i can use _env, database configuration inside class
            builder.Services.AddScoped<IAuthService, AuthService>();

            // Register custom logger so that i can use _env files inside the CustomLogger Class
            builder.Services.AddSingleton<ICustomLogger, CustomLogger>();

        }
    }
}
