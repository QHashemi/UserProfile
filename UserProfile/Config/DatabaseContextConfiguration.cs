using Microsoft.EntityFrameworkCore;
using UserProfile.Data;

namespace UserProfile.Config
{
    public static class DatabaseContextConfiguration
    {
        // use keywork this so that service apear in the list of services in program.cs
        public static void AddAppDbContextConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString")));
        }
    }
}
