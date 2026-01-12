using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Runtime.CompilerServices;
using UserProfile.HealthCheck;

namespace UserProfile.Config
{
    public static class HealthCheckConfiguration
    {
        public static void AddAllHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                 .AddCheck<RandomHealthCheck>("Random", tags: ["random"])
                 .AddCheck<HealthyHealthCheck>("Healthy", tags: ["healthy"])
                 .AddCheck<DegradedHealthCheck>("Degraded", tags: ["degraded"])
                 .AddCheck<UnhealthyHealthCheck>("Unhealthy", tags: ["unhealthy"]);

        }

        public static void MapAllHealthChecks(this WebApplication app)
        {
            // Check the healthy
            app.MapHealthChecks("/health");
            app.MapHealthChecks("/health/healthy", new HealthCheckOptions
            {
                Predicate = x => x.Tags.Contains("healthy")
            });
            app.MapHealthChecks("/health/degraded", new HealthCheckOptions
            {
                Predicate = x => x.Tags.Contains("degraded")
            });
            app.MapHealthChecks("/health/unhealthy", new HealthCheckOptions
            {
                Predicate = x => x.Tags.Contains("unhealthy")
            });
            app.MapHealthChecks("/health/random", new HealthCheckOptions
            {
                Predicate = x => x.Tags.Contains("random")
            });

            // UI Response in form of JSON
            app.MapHealthChecks("/health/ui", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.MapHealthChecks("/health/ui/healthy", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.MapHealthChecks("/health/ui/degraded", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.MapHealthChecks("/health/ui/unhealthy", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.MapHealthChecks("/health/ui/random", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });


        }
    }
}
