using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly DaprClient _daprClient;

        public WelcomeController(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(string name)
        {
            await _daprClient.PublishEventAsync("messages", "audit", new AuditInfo
            {
                Who = name,
                When = DateTimeOffset.UtcNow,
                What = nameof(WelcomeController)
            });
            return Ok(new { Msg = $"Welcome {name}! Hello from Dapr." });
        }

        public class AuditInfo
        {
            public string Who { get; set; }
            public DateTimeOffset When { get; set; }
            public string What { get; set; }
        }
    }
}
