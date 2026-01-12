using AspNetCoreRateLimit;

namespace UserProfile.Config
{
    public static class EndPointRateLimitConfiguration
    {
        public static void EndPointRateLimitConfig(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(options =>
            {
                options.GeneralRules = new List<RateLimitRule>{
                    new RateLimitRule
                    {
                        Endpoint = "*", // all endpoints
                        Limit = 20,    // 10 requests
                        Period = "1m"   // per 1 minute
                    }};
            });

            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}
