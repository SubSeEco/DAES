using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DAES.API.BackOffice.Modelos; // Asegúrate de incluir el espacio de nombres de tus modelos

namespace App.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RazonSocialController : ControllerBase
    {
        private readonly MyDbContext _dbContext;

        public RazonSocialController(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }
       
        // GET api/RazonSocial/{RS}
        [HttpGet("{RS}")]
        [Produces("application/json")]
        public IActionResult Get(string RS)
        {
            // Puedes realizar operaciones en la base de datos utilizando _dbContext
            var datos = _dbContext.TuTabla.Where(q => q.RazonSocial == RS).Any();

            if (datos)
            {
                return Ok($"{{\"RazonSocial\": \"{RS}\"}}");

               
            }
            else {
                return NotFound();
            }

            
            
        }
    }

    // Configuración de servicios en Startup.cs
    
}