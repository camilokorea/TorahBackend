using MongoDB.Driver;
using System.Text.Json;
using TorahBackend.Application.Interfaces;
using TorahBackend.Domain.Entities;

namespace TorahBackend.Infrastructure.Repositories
{
    public class TorahJsonRepository : ITorahJsonRepository
    {
        public async Task<List<Libro>> LoadData()
        {
            try
            {
                List<Libro> libros = new List<Libro>();

                var jsonData = await File.ReadAllTextAsync("DataSeed/torah.json");
                var librosDeserialize = JsonSerializer.Deserialize<List<Libro>>(jsonData);

                if (librosDeserialize != null) 
                {
                    libros = librosDeserialize;
                }

                return libros;
            }
            catch
            {
                throw;
            }
        }
    }
}
