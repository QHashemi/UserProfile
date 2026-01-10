using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserProfile.Data;
using UserProfile.Middleware;
using UserProfile.Services;
using UserProfile.Utils;


// SERVICES =========================================================================================>
var builder = WebApplication.CreateBuilder(args);
// Register Controllers
builder.Services.AddControllers();

//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

// Sql Server connection 
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));


// Secure end point configuration with jwt
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("AppSettings:Token")!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration.GetValue<string>("AppSettings:Issuer"),
        ValidAudience = builder.Configuration.GetValue<string>("AppSettings:Audience"),
    });


// Register Controllers so that i can use _env, database configuration inside class
builder.Services.AddScoped<IAuthService, AuthService>();

// Register custom logger so that i can use _env files inside the CustomLogger Class
builder.Services.AddSingleton<ICustomLogger, CustomLogger>();



// MIDLLEWARES ===========================================================================================>
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // only in production
}


// 1️⃣ Exception handling must be first
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 2️⃣ Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// 3️⃣ Security headers
app.UseMiddleware<SecureHeaderMiddleware>();

// 4️⃣ Log requests (metadata only)
app.UseMiddleware<RequestLoggingMiddleware>();


// Configure the HTTP request pipeline. 
//if (app.Environment.IsDevelopment())
//{ 
//app.MapOpenApi(); 
//app.MapScalarApiReference(); 
//}


// 5️⃣ Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 6️⃣ Map controllers
app.MapControllers();

app.Run();
