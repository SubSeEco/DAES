using System.Text.Json;
using NJsonSchema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NJsonSchema.Validation;

namespace App.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistroOrganizacionBOController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        public RegistroOrganizacionBOController(IMemoryCache cache)
        {
            _cache = cache;
        }

        // POST api/RegistroOrganizacionBO
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Post([FromBody] JsonDocument jsonDocument)
        {
            try
            {
                _cache.TryGetValue("Organizacion", out string organizacionSchema);
                var schema = JsonSchema.FromJsonAsync(organizacionSchema).Result;
                var validator = new JsonSchemaValidator();
                var validation = validator.Validate(jsonDocument.RootElement.ToString(), schema);
                if (validation.Count != 0) { return BadRequest("json invalido"); }

                RegistroOrganizacionBO registroOrganizacionBO = jsonDocument.Deserialize<RegistroOrganizacionBO>();
                
                return Ok(registroOrganizacionBO.ContactoDeLaCooperativa.EMail);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
