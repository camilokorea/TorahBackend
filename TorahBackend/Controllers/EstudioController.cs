using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorahBackend.Application.DTO;
using TorahBackend.Application.Services;
using TorahBackend.Domain.Entities;

namespace TorahBackend.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class EstudioController : ControllerBase
    {
        private readonly IEstudioService _estudioService;

        public EstudioController(IEstudioService estudioService)
        {
            _estudioService = estudioService;
        }

        // Estudios publicados, completos, para la app pública.
        [HttpGet("publicados")]
        public async Task<IActionResult> Publicados()
        {
            return Ok(await _estudioService.ListarPublicados());
        }

        [Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            return Ok(await _estudioService.ListarTodos());
        }

        [Authorize]
        [HttpGet("get")]
        public async Task<IActionResult> Get(string id)
        {
            var estudio = await _estudioService.Obtener(id);
            return estudio == null ? NotFound(new { message = "No se encontró el estudio." }) : Ok(estudio);
        }

        [Authorize]
        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] EstudioCrear payload)
        {
            return Ok(await _estudioService.Crear(payload?.Titulo ?? string.Empty));
        }

        [Authorize]
        [HttpPost("guardar")]
        public async Task<IActionResult> Guardar([FromBody] EstudioGuardar payload)
        {
            if (payload == null) return BadRequest(new { message = "Solicitud inválida." });

            var r = await _estudioService.Guardar(payload);
            return r.Resultado switch
            {
                ResultadoOperacion.Ok => Ok(r.Estudio),
                ResultadoOperacion.NoEncontrado => NotFound(new { message = "No se encontró el estudio." }),
                ResultadoOperacion.Conflicto => Conflict(new { message = r.Mensaje }),
                _ => BadRequest(new { message = r.Mensaje }),
            };
        }

        [Authorize]
        [HttpPost("estado")]
        public async Task<IActionResult> Estado([FromBody] EstudioEstado payload)
        {
            return await _estudioService.CambiarEstado(payload?.Id ?? string.Empty, payload?.Publicado ?? false)
                ? Ok(true)
                : NotFound(new { message = "No se encontró el estudio." });
        }

        [Authorize]
        [HttpPost("eliminar")]
        public async Task<IActionResult> Eliminar([FromBody] EstudioId payload)
        {
            return await _estudioService.Eliminar(payload?.Id ?? string.Empty)
                ? Ok(true)
                : NotFound(new { message = "No se encontró el estudio." });
        }

        [Authorize]
        [HttpPost("ordenar")]
        public async Task<IActionResult> Ordenar([FromBody] EstudiosOrden payload)
        {
            await _estudioService.Ordenar(payload?.Ids ?? new List<string>());
            return Ok(true);
        }
    }
}
