using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace Supersonic.Registry.Controllers
{
    [ApiController]
    [Route("api/registry")]
    public class RegistryController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, string> Nodes = new ConcurrentDictionary<string, string>();
        private readonly ILogger<RegistryController> _logger;

        public RegistryController(ILogger<RegistryController> logger)
        {
            _logger = logger;
        }

        [HttpPost("register")]
        public IActionResult RegisterNode([FromBody] string nodeUrl)
        {
            _logger.LogInformation("Registering node: {NodeUrl}", nodeUrl);
            Nodes[nodeUrl] = nodeUrl;
            _logger.LogInformation("Node registered successfully: {NodeUrl}", nodeUrl);
            return Ok();
        }

        [HttpGet("nodes")]
        public IActionResult GetNodes()
        {
            _logger.LogInformation("Getting list of registered nodes.");
            var nodeList = Nodes.Values;
            _logger.LogInformation("Number of registered nodes: {Count}", nodeList.Count());
            return Ok(nodeList);
        }
    }
}
