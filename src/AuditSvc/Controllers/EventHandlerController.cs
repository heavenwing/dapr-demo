using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AuditSvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventHandlerController : ControllerBase
    {
        [Topic("messages", "audit")]
        [HttpPost("auditplaced")]
        public async Task<IActionResult> PlaceAudit(AuditInfo info, [FromServices] DaprClient dapr, [FromServices] ILogger<EventHandlerController> logger)
        {
            logger.LogInformation("Got audit {Who} at {When} from {What}", info.Who, info.When, info.What);

            if (string.IsNullOrEmpty(info?.Who))
            {
                logger.LogWarning("Audit info's who is null or empty");
                return Ok(new { status = "DROP" });
            }

            try
            {
                await dapr.SaveStateAsync("statestore", info.Who, info);
                logger.LogInformation("Saved audit info into state management {Who}", info.Who);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failure saving audit info into state management {Who}", info.Who);
                return NotFound();
            }
        }
    }
}
