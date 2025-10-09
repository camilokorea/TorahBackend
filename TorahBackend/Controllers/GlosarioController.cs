using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorahBackend.Application.DTO;
using TorahBackend.Application.Interfaces;

namespace TorahBackend.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class GlosarioController : Controller
    {
        private readonly IGlosarioService _glosarioService;

        public GlosarioController(IGlosarioService glosarioService) 
        {
            _glosarioService = glosarioService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            var glosarios = await _glosarioService.List();
            return Ok(glosarios);
        }

        [Authorize]
        [HttpPatch("actualizar/contenido")]
        public async Task<IActionResult> ActualizarContenido([FromBody] GlosarioDTO payload)
        {
            await _glosarioService.Actualizar(payload.Id, payload.Contenido);
            return Ok(true);
        }
    }
}
