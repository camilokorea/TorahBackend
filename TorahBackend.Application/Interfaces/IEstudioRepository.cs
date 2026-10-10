using TorahBackend.Domain.Entities;

namespace TorahBackend.Application.Interfaces
{
    public interface IEstudioRepository
    {
        Task<List<Estudio>> Listar(bool soloPublicados);

        Task<Estudio?> Obtener(string id);

        Task Insertar(Estudio estudio);

        // Reemplaza el estudio solo si su revisión sigue siendo revisionEsperada.
        Task<bool> Reemplazar(Estudio estudio, int revisionEsperada);

        Task<bool> CambiarEstado(string id, bool publicado);

        Task<bool> Eliminar(string id);

        Task ActualizarOrden(IReadOnlyList<string> ids);

        Task<bool> LibroExiste(string libroId);
    }

    public interface IImagenRepository
    {
        Task<string> Subir(Stream contenido, string nombre, string tipoContenido);

        Task<(byte[] Datos, string TipoContenido)?> Obtener(string id);

        Task Eliminar(string id);
    }
}
