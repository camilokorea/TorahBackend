using TorahBackend.Application.Interfaces;
using TorahBackend.Domain.Entities;
using TorahBackend.Application.DTO;

namespace TorahBackend.Application.Services
{
    public class DedicatoriaService : IDedicatoriaService
    {
        private readonly IDataRepository _dataRepository;

        private readonly IVersionControladorService _versionControladorService;
        public DedicatoriaService(IDataRepository dataRepository, IVersionControladorService versionControladorService)
        {
            _dataRepository = dataRepository;
            _versionControladorService = versionControladorService;
        }

        public async Task<List<DedicatoriaDTO>> List()
        {
            try
            {
                var list = new List<DedicatoriaDTO>();
                var dedicatoriaRecords = await _dataRepository.ListDedicatoria();

                if (dedicatoriaRecords != null)
                {
                    dedicatoriaRecords.ForEach(dedicatoria =>
                    {
                        list.Add(new DedicatoriaDTO
                        {
                            Id = dedicatoria.Id,
                            Contenido = dedicatoria.Contenido,
                        });
                    });
                }

                return list;
            }
            catch
            {
                throw;
            }
        }
        public async Task Actualizar(string? id, string? contenido)
        {
            try
            {
                await _dataRepository.UpdateDedicatoria(id, contenido);
                await _versionControladorService.IncrementarVersion();
            }
            catch
            {
                throw;
            }
        }

    }
}
