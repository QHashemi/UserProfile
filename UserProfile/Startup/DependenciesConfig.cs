using UserProfile.Config;
using UserProfile.Services;
using UserProfile.Services.AuthService;
using UserProfile.Services.EmailService;
using UserProfile.Services.LoggerService;
using UserProfile.Services.UserServices;

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

            // Inject appsettings.json to the service, so that the service can use the settings from appsettings.json
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));


            // Register application services for dependency injection
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
          

            // Register custom logger so that i can use _env files inside the CustomLogger Class
            builder.Services.AddSingleton<ICustomLoggerService, CustomLoggerService>();

        }
    }
}
