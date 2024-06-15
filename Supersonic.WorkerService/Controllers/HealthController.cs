using Microsoft.AspNetCore.Mvc;

namespace Supersonic.WorkerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok("Service is healthy");
        }
    }
}
