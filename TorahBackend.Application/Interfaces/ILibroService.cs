using TorahBackend.Application.DTO;
using TorahBackend.Domain.Entities;

namespace TorahBackend.Application.Interfaces
{
    public interface ILibroService
    {
        Task <List<LibroInfo>> List();

        Task<LibroDetalle> Get(string? id);

        Task<Torah> GetTorah();

        Task<List<string>> GetTestamentos();

        Task ActualizarNombre(string? id, string? nombre);

        Task ActualizarAbreviatura(string? id, string? abreviatura);

        Task ActualizarVersiculo(string? id, int capituloNumero, int versiculoNumero, string? versiculo);

        Task<ResultadoOperacion> InsertarVersiculo(string? id, int capituloNumero, int posicion, string versiculo);

        Task<ResultadoOperacion> EliminarVersiculo(string? id, int capituloNumero, int posicion, string versiculoActual);
    }
}
