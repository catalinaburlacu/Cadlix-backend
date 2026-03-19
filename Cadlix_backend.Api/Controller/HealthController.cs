using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cadlix_backend.Api.Controller
{
    [Route("api/health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API is healthy");
        }
    }
}