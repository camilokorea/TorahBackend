using TorahBackend.Application.DTO;

namespace TorahBackend.Application.Interfaces
{
    public interface IDedicatoriaService
    {
        Task<List<DedicatoriaDTO>> List();
        Task Actualizar(string? id, string? contenido);
    }
}
