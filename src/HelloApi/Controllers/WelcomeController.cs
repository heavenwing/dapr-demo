using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WelcomeController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync(string name, [FromServices] DaprClient dapr, [FromServices] ILogger<WelcomeController> logger)
        {
            logger.LogInformation("Got welcome to {Name}", name);

            await dapr.PublishEventAsync("messages", "audit", new AuditInfo
            {
                Who = name,
                When = DateTimeOffset.UtcNow,
                What = nameof(WelcomeController)
            });
            logger.LogInformation("Published audit info {Who}", name);

            return Ok(new { Msg = $"Welcome {name}! Hello from Dapr." });
        }
    }
}
