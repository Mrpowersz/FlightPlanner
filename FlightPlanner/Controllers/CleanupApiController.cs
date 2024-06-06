using FlightPlanner.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Route("testing-api")]
    [ApiController]
    public class CleanupApiController(ICleanupService cleanupService) : ControllerBase
    {
        private readonly ICleanupService _cleanupService = cleanupService;

        [HttpPost]
        [Route("clear")]
        public IActionResult Clear()
        {
            _cleanupService.ClearData();
            return Ok();
        }
    }
}

