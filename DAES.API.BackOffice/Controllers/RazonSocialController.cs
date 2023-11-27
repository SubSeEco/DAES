using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RazonSocialController : ControllerBase
    {
        // GET api/RazonSocial/{RS}
        [HttpGet("{RS}")]
        [Produces("application/json")]
        public IActionResult Get(string RS)
        {
            if (false)
            {
                return BadRequest("Razon");
            }
            return Ok($"{{\"RazonSocial\": \"{RS}\"}}");
        }
    }
}
