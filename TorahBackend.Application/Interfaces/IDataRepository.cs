
using TorahBackend.Domain.Entities;

namespace TorahBackend.Application.Interfaces
{
    public interface IDataRepository
    {
        Task DataSeed();
        Task<Usuario> GetUsuario(string? email);
        Task<List<string>> ListTestamentos();
        Task<List<Libro>> ListLibros();
        Task<Libro> ListLibro(string? id);        
        Task UpdateNombreLibro(string? id, string? nombre);
        Task UpdateAbreviaturaLibro(string? id, string? abreviatura);
        Task UpdateVersiculoLibro(string? id, int capituloNumero, int versiculoNumero, string? versiculo);
        Task IncremetarVersion();
        Task<VersionControlador> ObtenerUltimaVersion();
        Task<List<Glosario>> ListGlosario();
        Task<List<Dedicatoria>> ListDedicatoria();
        Task UpdateGlosario(string? id, string? contenido);
        Task UpdateDedicatoria(string? id, string? contenido);
    }
}
