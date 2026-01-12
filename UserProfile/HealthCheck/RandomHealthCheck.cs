using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Reflection.Metadata.Ecma335;

namespace UserProfile.HealthCheck
{
    public class RandomHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
          CancellationToken cancellationToken = default)
        {
            int randomResult = Random.Shared.Next(1, 4);

            return randomResult switch
            {
                1 => Task.FromResult(HealthCheckResult.Healthy("This is test Healthy Service.")),
                2 => Task.FromResult(HealthCheckResult.Degraded("This is test Degraded Service.")),
                3 => Task.FromResult(HealthCheckResult.Unhealthy("This is test Unhealthy Service.")),
                _ => Task.FromResult(HealthCheckResult.Healthy("This is test Healthy Service.")),
            };
        }

    }
}
