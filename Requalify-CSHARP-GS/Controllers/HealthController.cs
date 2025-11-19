using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Challenge_MOTTU.Controllers
{
    /// <summary>
    /// Controller responsible for API and database health monitoring.
    /// </summary>
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Checks the overall health status of the API and the Oracle database.
        /// </summary>
        /// <remarks>
        /// Returns the current status of the application and its dependencies (e.g., database connection).
        /// </remarks>
        /// <returns>Returns status "Healthy" if everything is working correctly.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 503)]
        public async Task<IActionResult> GetHealthStatus([FromServices] HealthCheckService healthCheckService)
        {
            var report = await healthCheckService.CheckHealthAsync();

            var result = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description
                })
            };

            if (report.Status == HealthStatus.Healthy)
                return Ok(result);

            return StatusCode(503, result);
        }
    }
}
