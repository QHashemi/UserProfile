using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace UserProfile.Config
{
    public static class Jwt 
    {
        public static void AddAuthenticationJwt(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration.GetValue<string>("AppSettings:Issuer"),
                    ValidAudience = configuration.GetValue<string>("AppSettings:Audience"),
                });
               }
    }
}
