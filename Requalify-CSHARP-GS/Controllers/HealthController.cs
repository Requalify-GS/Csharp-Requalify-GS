using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Challenge_MOTTU.Controllers
{
    /// <summary>
    /// Controller para gerenciamento da API e Banco
    /// </summary>
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Verifica o status geral da API e do banco de dados Oracle.
        /// </summary>
        /// <remarks>
        /// Retorna o estado atual da aplicação e suas dependências (ex: conexão com o banco).
        /// </remarks>
        /// <returns>Status "Healthy" se tudo estiver funcionando corretamente.</returns>
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
