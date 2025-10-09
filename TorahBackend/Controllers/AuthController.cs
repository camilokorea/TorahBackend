using Microsoft.AspNetCore.Mvc;
using TorahBackend.Application.DTO;
using TorahBackend.Application.Interfaces;

namespace TorahBackend.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AuthController: ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public AuthController(IUsuarioService usuarioService) 
        { 
            _usuarioService = usuarioService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "Correo y contraseña son requeridos." });

            var user = await _usuarioService.GetUsuario(request.Email);

            if (!_usuarioService.Authenticate(user, request.Password))
            {
                return Unauthorized(new { message = "Credenciales incorrectas." });
            }
            else {
                var token = _usuarioService.GenerateJwtToken(user);

                return Ok(new { token });
            }
        }
    }
}
