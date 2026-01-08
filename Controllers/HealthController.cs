using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserProfile.Data;

namespace UserProfile.Controllers
{
    public static class AppLifetime
    {
        public static readonly DateTime StartTime = DateTime.UtcNow;
    }

    [ApiController]
    [Route("")]
    public class HealthController(AppDbContext context) : ControllerBase
    {
        /// <summary>
        /// Basic API connectivity test.
        /// </summary>
        [HttpGet("ping")]
        public ActionResult<string> Ping()
        {
            return "Hello from Production API!";
        }

        /// <summary>
        /// Full health check including app uptime.
        /// </summary>
        [HttpGet("health")]
        public ActionResult<object> GetHealth()
        {
            TimeSpan uptime = DateTime.UtcNow - AppLifetime.StartTime;

            var healthStatus = new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                uptime = uptime
            };

            return Ok(healthStatus);
        }


        /// <summary>
        /// General API status check.
        /// </summary>
        [HttpGet("status")]
        public ActionResult<string> GetApiStatus()
        {
            return "Production API is running smoothly";
        }

        /// <summary>
        /// Database connectivity check.
        /// </summary>
        [HttpGet("database")]
        public async Task<ActionResult<string>> CheckDatabase()
        {
            // check database connection
            try
            {
                var canConnect = await context.Database.CanConnectAsync();
                if (canConnect) 
                {
                    return Ok("Database connection is healthy");
                }
                else
                {
                    return StatusCode(503, "Database connection failed");

                }
            }
            catch (Exception ex) {
                return StatusCode(500, $"Database check failed: {ex.Message}");
            }
        }



        /// <summary>
        /// Readiness probe for container orchestration.
        /// </summary>
        [HttpGet("ready")]
        public ActionResult<string> Readiness()
        {
            return "Application is ready";
        }

        /// <summary>
        /// Liveness probe for container orchestration.
        /// </summary>
        [HttpGet("live")]
        public ActionResult<string> Liveness()
        {
            return "Application is live";
        }
    }
}
