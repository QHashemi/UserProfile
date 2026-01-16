using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserProfile.Controllers;
using UserProfile.Data;
using UserProfile.Dto.Response;
using Moq;
using UserProfile.Services.LoggerService;

namespace UserProfile.Test.Controllers
{
    public class ApiHealthTest
    {
        private readonly DbContextOptionsBuilder<AppDbContext> dbContext = new();


        // Create fake custom Logger With Moq
        private static ICustomLoggerService CreateFakeLogger()
        {
            return Mock.Of<ICustomLoggerService>();
        }

        [Fact]
        public void HealthTest()
        {
            // Arrange ================>
            var healthController = new ApiHealthController(ReturnContext(), CreateFakeLogger());

            // Act ==================>
            var actionResult = healthController.GetHealth();

            // Assert ==================>
            var okResult = actionResult.Result as ObjectResult; // unwrap safely
            var healtResult = okResult?.Value as TestControllersResponseDto;
            Assert.NotNull(healtResult);
            Assert.Equal("Healthy", healtResult.Status);
        }

        [Fact]
        public void PingTest()
        {
            // Arrange =================>
            var pingController = new ApiHealthController(ReturnContext(), CreateFakeLogger());

            // Act =====================
            var actionResult = pingController.Ping();

            // Assert ===================
            var okResult = actionResult.Result as ObjectResult;
            var pingResult = okResult?.Value as TestControllersResponseDto;

            Assert.NotNull(pingResult);
            Assert.Equal("Healthy", pingResult.Status);
        }

        [Fact]
        public void ReadyTest()
        {
            // Arrange =================>
            var pingController = new ApiHealthController(ReturnContext(), CreateFakeLogger());

            // Act =====================
            var actionResult = pingController.Readiness();

            // Assert ===================
            var okResult = actionResult.Result as ObjectResult;
            var pingResult = okResult?.Value as TestControllersResponseDto;

            Assert.NotNull(pingResult);
            Assert.Equal("Healthy", pingResult.Status);
        }

        [Fact]
        public void LiveTest()
        {
            // Arrange =================>
            var pingController = new ApiHealthController(ReturnContext(), CreateFakeLogger());

            // Act =====================
            var actionResult = pingController.Liveness();

            // Assert ===================
            var okResult = actionResult.Result as ObjectResult;
            var pingResult = okResult?.Value as TestControllersResponseDto;

            Assert.NotNull(pingResult);
            Assert.Equal("Healthy", pingResult.Status);
        }

        public void DatabaseTest()
        {
            // Arrange =================>
            var pingController = new ApiHealthController(ReturnContext(), CreateFakeLogger());

            // Act =====================
            var actionResult = pingController.CheckDatabase();

            // Assert ===================
            var okResult = actionResult.Result;
            var pingResult = okResult?.Value as TestControllersResponseDto;

            Assert.NotNull(pingResult);
            Assert.Equal("Healthy", pingResult.Status);
        }

        // Get Fake DB
        private AppDbContext ReturnContext()
        {
            var options = dbContext.UseInMemoryDatabase(databaseName: "TestDb").Options;
            using var context = new AppDbContext(options);
            return context;
        }
    }
}
