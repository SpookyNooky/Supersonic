using Microsoft.AspNetCore.Mvc;
using Supersonic.Infrastructure.Network;

namespace Supersonic.WorkerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly NetworkService _networkService;

        public TransactionController(NetworkService networkService)
        {
            _networkService = networkService;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            await _networkService.ReceiveTransactionAsync(HttpContext);
            return Ok();
        }
    }
}
