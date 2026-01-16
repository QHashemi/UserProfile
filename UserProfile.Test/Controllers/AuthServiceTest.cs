
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using UserProfile.Controllers;
using UserProfile.Data;
using UserProfile.Dto.Request;
using UserProfile.Dto.Response;
using UserProfile.Services.AuthService;
using UserProfile.Services.EmailService;
using UserProfile.Services.LoggerService;

namespace UserProfile.Tests.Controllers
{
    public class AuthControllerTests
    {
        // Create Fake of every dependency
        private AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("AuthTestDb")
                .Options;

            return new AppDbContext(options);
        }

        private static ICustomLoggerService CreateFakeLogger()
        {
            return Mock.Of<ICustomLoggerService>();
        }

        private static IConfiguration CreateFakeConfiguration()
        {
            return new ConfigurationBuilder().Build();
        }

        private static IHttpContextAccessor CreateFakeHttpContext()
        {
            return new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        private static IEmailService CreateFakeEmailService()
        {
            return Mock.Of<IEmailService>();
        }


        private IAuthService CreateAuthService()
        {
            return new AuthService(
                CreateDbContext(),
                CreateFakeConfiguration(),
                CreateFakeLogger(),
                CreateFakeHttpContext(),
                CreateFakeEmailService()   
            );
        }




        [Fact]
        public async Task Valid_User_Regestration()
        {
            // Arrange
            var registerRequest = new RegisterRequestDto
            {
                FirstName = "Jahn",
                LastName = "Doe",
                Email = "jahn.doe@gmail.com",
                Password = "user12345",
                ConfirmPassword = "user12345"
            };

            var authService = CreateAuthService();
            var controller = new AuthController(authService);

            // Act
            var actionResult = controller.Register(registerRequest);

            // Assert
            var okResult = actionResult.Result.Result as ObjectResult;
            Assert.NotNull(okResult);

            var response = okResult.Value as RegisterResponseDto;
            Assert.NotNull(response);
            Assert.Equal("jahn.doe@gmail.com", response.User.Email);
            
        }
    }
}
