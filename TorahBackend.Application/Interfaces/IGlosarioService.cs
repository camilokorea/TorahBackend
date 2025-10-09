using TorahBackend.Application.DTO;

namespace TorahBackend.Application.Interfaces
{
    public interface IGlosarioService
    {
        Task<List<GlosarioDTO>> List();
        Task Actualizar(string? id, string? contenido);
    }
}
