using System.Text.Json;
using NJsonSchema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NJsonSchema.Validation;

namespace App.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreacionProcesoRESController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        public CreacionProcesoRESController(IMemoryCache cache)
        {
            _cache = cache;
        }
        // POST api/ConstitucionCooperativas
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Post([FromBody] JsonDocument jsonDocument)
        {
            try
            {
                _cache.TryGetValue("Proceso", out string procesoSchema);
                var schema = JsonSchema.FromJsonAsync(procesoSchema).Result;
                var validator = new JsonSchemaValidator();
                var validation = validator.Validate(jsonDocument.RootElement.ToString(), schema);
                if (validation.Count != 0) { return BadRequest("json invalido"); }

                RegistroOrganizacionBO creacionProcesoRES = jsonDocument.Deserialize<RegistroOrganizacionBO>();

                return Ok(creacionProcesoRES.ContactoDeLaCooperativa.EMail);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
