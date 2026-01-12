
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
using UserProfile.Utils.Interfaces;

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

        private static ICustomLogger CreateFakeLogger()
        {
            return Mock.Of<ICustomLogger>();
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

        private IAuthService CreateAuthService()
        {
            return new AuthService(
                CreateDbContext(),
                CreateFakeConfiguration(),
                CreateFakeLogger(),
                CreateFakeHttpContext()
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
