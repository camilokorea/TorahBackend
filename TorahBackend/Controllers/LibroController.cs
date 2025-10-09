using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorahBackend.Application.DTO;
using TorahBackend.Application.Interfaces;

namespace TorahBackend.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class LibroController: ControllerBase
    {
        private readonly ILibroService _libroService;

        public LibroController(ILibroService libroService) { 
            _libroService = libroService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            var libros = await _libroService.List();
            return Ok(libros);
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get(string id)
        {
            var libro = await _libroService.Get(id);

            if (libro == null) {
                return NotFound(new { message = $"El recurso con ID {id} no fue encontrado." });
            }

            return Ok(libro);
        }

        [HttpGet("get/torah")]
        public async Task<IActionResult> Torah()
        {
            var torah = await _libroService.GetTorah();
            return Ok(torah);
        }

        [HttpGet("list/testamento")]
        public async Task<IActionResult> Testamentos()
        {
            var testamentos = await _libroService.GetTestamentos();
            return Ok(testamentos);
        }

        [Authorize]
        [HttpPatch("actualizar/nombre")]
        public async Task<IActionResult> ActualizarNombre([FromBody] LibroNombre payload)
        {
            await _libroService.ActualizarNombre(payload.Id, payload.Nombre);
            return Ok(true);
        }

        [Authorize]
        [HttpPatch("actualizar/abreviatura")]
        public async Task<IActionResult> ActualizarAbreviatura([FromBody] LibroAbreviatura payload)
        {
            await _libroService.ActualizarAbreviatura(payload.Id, payload.Abreviatura);
            return Ok(true);
        }

        [Authorize]
        [HttpPatch("actualizar/versiculo")]
        public async Task<IActionResult> ActualizarVersiculo([FromBody] LibroVersiculo payload)
        {
            await _libroService.ActualizarVersiculo(payload.Id, payload.CapituloNumero, payload.VersiculoNumero, payload.Versiculo);
            return Ok(true);
        }
    }
}
