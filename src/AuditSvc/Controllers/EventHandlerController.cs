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
        public async Task PlaceAudit(AuditInfo info, [FromServices] DaprClient dapr, [FromServices] ILogger<EventHandlerController> logger)
        {
            logger.LogInformation("Got audit {Who} at {When} from {What}", info.Who,info.When,info.What);

            if (string.IsNullOrEmpty(info?.Who)) return;

            await dapr.SaveStateAsync("statestore", info.Who, info);
            logger.LogInformation("Saved audit info into state management {Who}", info.Who);
        }
    }
}
