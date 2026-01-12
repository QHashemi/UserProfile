using Scalar.AspNetCore;

namespace UserProfile.Config
{
    public static class OpenApiConfiguration
    {
        public static void AddOpenApiService(this IServiceCollection services)
        {
            services.AddOpenApi();
        }

        public static void UseOpenApi(this WebApplication app)
        {

            // Configure the HTTP request pipeline. 
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                // scalar configurtation
                app.MapScalarApiReference(options =>
                {
                    options.Title = "My User Profile Web Api";
                    options.Theme = ScalarTheme.Saturn;
                    options.Layout = ScalarLayout.Modern;
                    options.HideClientButton = true;
                });
               }
            }
    }
}
