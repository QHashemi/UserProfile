using Microsoft.AspNetCore.Mvc;
using UserProfile.Data;
using UserProfile.Dto.Response;
using UserProfile.Services.LoggerService;

namespace UserProfile.Controllers
{
    public static class AppLifetime
    {
        public static readonly DateTime StartTime = DateTime.UtcNow;
    }

    [ApiController]
    [Route("/api")]
    public class ApiHealthController(AppDbContext context, ICustomLoggerService logger) : ControllerBase
    {

        /// <summary>
        /// Basic API connectivity test.
        /// </summary>
        [HttpGet("ping")]
        public ActionResult<string> Ping()
        {
            return Ok(ResponseDto("Healthy"));
        }

        /// <summary>
        /// Full health check including app uptime.
        /// </summary>
        [HttpGet("health")]
        public ActionResult<object> GetHealth()
        {

            return Ok(ResponseDto("Healthy"));
        }

        /// <summary>
        /// General API status check.
        /// </summary>
        [HttpGet("status")]
        public ActionResult<TestControllersResponseDto> GetApiStatus()
        {
            return Ok(ResponseDto("Healthy"));
        }

        /// <summary>
        /// Database connectivity check.
        /// </summary>
        [HttpGet("database")]
        public async Task<ActionResult<TestControllersResponseDto>> CheckDatabase()
        {
            // check database connection
            try
            {
                TimeSpan uptime = DateTime.UtcNow - AppLifetime.StartTime;
                var canConnect = await context.Database.CanConnectAsync();
                if (canConnect) 
                {
                    return Ok(ResponseDto("Healthy"));
                }
                else
                {
                    await logger.Critical(message: "Unhealthy Database connection", logEvent:"DATABASE_HEALTH_TEST");
                    return ResponseDto("Unhealthy");
                }
            }
            catch (Exception ex) {
                await logger.Critical(message: "Database connection check failed", logEvent: "DATABASE_HEALTH_TEST");
                return StatusCode(500, $"Database check failed: {ex.Message}");
            }
        }


        /// <summary>
        /// Readiness probe for container orchestration.
        /// </summary>
        [HttpGet("ready")]
        public ActionResult<TestControllersResponseDto> Readiness()
        {
            TimeSpan uptime = DateTime.UtcNow - AppLifetime.StartTime;
            return Ok(ResponseDto("Healthy"));
        }

        /// <summary>
        /// Liveness probe for container orchestration.
        /// </summary>
        [HttpGet("live")]
        public ActionResult<TestControllersResponseDto> Liveness()
        {
            return Ok(ResponseDto("Healthy"));
        }



        // Method to return Response
        private TestControllersResponseDto ResponseDto(string status)
        {
            TimeSpan uptime = DateTime.UtcNow - AppLifetime.StartTime;
            return new TestControllersResponseDto
            {
                Status = status,
                TimeStamp = DateTime.UtcNow,
                Uptime = uptime
            };
        }
    }
}
