using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cadlix_backend.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpPost]
         public IActionResult GetAllUsers()
         {
             return Ok("OK");
         }
    }
}
