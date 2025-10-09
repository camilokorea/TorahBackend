using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorahBackend.Application.DTO;
using TorahBackend.Application.Interfaces;
using TorahBackend.Application.Services;

namespace TorahBackend.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class VersionControladorController : ControllerBase
    {
        private readonly IVersionControladorService _versionControladorService;

        public VersionControladorController(IVersionControladorService versionControladorService)
        {
            _versionControladorService = versionControladorService;
        }

        [HttpGet("ultimaversion")]
        public async Task<IActionResult> ObtenerUltimaVersion()
        {
            var ultimaVersion = await _versionControladorService.ObtenerUltimaVersion();
            return Ok(ultimaVersion);
        }
    }
}
