using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorahBackend.Application.Interfaces;

namespace TorahBackend.WebApi.Controllers
{
    // Imágenes de los estudios (guardadas en MongoDB/GridFS).
    [Route("api/[controller]")]
    public class ImagenController : ControllerBase
    {
        private const long MaxBytes = 8 * 1024 * 1024;
        private static readonly HashSet<string> Permitidos = new() { "image/jpeg", "image/png", "image/webp", "image/gif" };

        private readonly IImagenRepository _imagenes;

        public ImagenController(IImagenRepository imagenes)
        {
            _imagenes = imagenes;
        }

        [Authorize]
        [HttpPost("subir")]
        [RequestSizeLimit(MaxBytes + 64 * 1024)]
        public async Task<IActionResult> Subir(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0) return BadRequest(new { message = "No se recibió ninguna imagen." });
            if (archivo.Length > MaxBytes) return BadRequest(new { message = "La imagen no puede pasar de 8 MB." });
            if (!Permitidos.Contains(archivo.ContentType)) return BadRequest(new { message = "Formato no permitido. Usa JPG, PNG, WebP o GIF." });

            await using var stream = archivo.OpenReadStream();
            var id = await _imagenes.Subir(stream, Path.GetFileName(archivo.FileName), archivo.ContentType);
            return Ok(new { id });
        }

        // El contenido de una imagen nunca cambia (cada subida tiene un id nuevo), así que se puede cachear para siempre.
        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(string id)
        {
            var imagen = await _imagenes.Obtener(id);
            if (imagen == null) return NotFound();
            Response.Headers["Cache-Control"] = "public, max-age=31536000, immutable";
            return File(imagen.Value.Datos, imagen.Value.TipoContenido);
        }
    }
}
