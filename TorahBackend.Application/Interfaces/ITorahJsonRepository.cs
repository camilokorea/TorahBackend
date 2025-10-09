
using TorahBackend.Domain.Entities;

namespace TorahBackend.Application.Interfaces
{
    public interface ITorahJsonRepository
    {
        Task<List<Libro>> LoadData();
    }
}
