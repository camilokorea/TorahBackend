using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorahBackend.Application.DTO;
using TorahBackend.Application.Interfaces;

namespace TorahBackend.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class DedicatoriaController : Controller
    {
        private readonly IDedicatoriaService _dedicatoriaService;

        public DedicatoriaController(IDedicatoriaService dedicatoriaService)
        {
            _dedicatoriaService = dedicatoriaService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            var dedicatorias = await _dedicatoriaService.List();
            return Ok(dedicatorias);
        }

        [Authorize]
        [HttpPatch("actualizar/contenido")]
        public async Task<IActionResult> ActualizarContenido([FromBody] DedicatoriaDTO payload)
        {
            await _dedicatoriaService.Actualizar(payload.Id, payload.Contenido);
            return Ok(true);
        }
    }
}
