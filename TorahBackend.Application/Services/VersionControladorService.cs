using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorahBackend.Application.DTO;
using TorahBackend.Application.Interfaces;

namespace TorahBackend.Application.Services
{
    public class VersionControladorService: IVersionControladorService
    {
        private readonly IDataRepository _dataRepository;

        public VersionControladorService(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public async Task IncrementarVersion() 
        {
            try
            {
                await _dataRepository.IncremetarVersion();
            }
            catch
            {
                throw;
            }
        }

        public async Task<VersionControlador> ObtenerUltimaVersion()
        {
            try 
            {
                var ultimaVersion = await _dataRepository.ObtenerUltimaVersion();

                return new VersionControlador { 
                    Id = ultimaVersion.Id,
                    Timestamp = ultimaVersion.Timestamp,
                    Version = ultimaVersion.Version
                };
            }
            catch
            {
                throw;
            }
        }
    }
}
