using Microsoft.EntityFrameworkCore;
using UserProfile.Data;

namespace UserProfile.Config
{
    public static class DatabaseContextConfiguration
    {
        // use keywork this so that service apear in the list of services in program.cs
        public static void AddAppDbContextConfig(this IServiceCollection services, IConfiguration configuration)
        {
            // Connect to postgreSQL ============================================================>
            //var DB_HOST = "db";
            //var DB_USER = "admin";
            //var DB_NAME = "userprofile";
            //var DB_PORT = 5432;
            //var DB_PASSWORD = "Admin4320!";
            //var connectionString = $"Host={DB_HOST};Port={DB_PORT};Database={DB_NAME};Username={DB_USER};Password={DB_PASSWORD};";
            //services.AddDbContext<AppDbContext>(options =>
            //    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
            //    {
            //        npgsqlOptions.EnableRetryOnFailure(
            //            maxRetryCount: 5,
            //            maxRetryDelay: TimeSpan.FromSeconds(10),
            //            errorCodesToAdd: null
            //        );
            //    }));


            // Connection to Microsoft SQL SERVER ==================================================>
            // Access db connection string from setting inside the appsettting.json
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString")));
        }

        // Create table and migration
        public static void MigrateDatabase(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var pendingMigrations = db.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                Console.WriteLine("Applying pending migrations...");
                db.Database.Migrate();
            }
            else
            {
                Console.WriteLine("Database up to date, no migrations needed.");
            }
        }


    }


}
                         