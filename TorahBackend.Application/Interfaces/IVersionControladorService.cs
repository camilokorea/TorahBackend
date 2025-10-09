using TorahBackend.Application.DTO;

namespace TorahBackend.Application.Interfaces
{
    public interface IVersionControladorService
    {
        Task IncrementarVersion();

        Task<VersionControlador> ObtenerUltimaVersion();
    }
}
