using TorahBackend.Application.Interfaces;
using MongoDB.Driver;
using TorahBackend.Domain.Entities;
using MongoDB.Bson;
using System.Reflection.Metadata;

namespace TorahBackend.Infrastructure.Repositories
{
    public class DataRepository : IDataRepository
    {
        private readonly string _connectionString;
        private readonly IMongoDatabase? _database;
        private readonly IMongoCollection<Libro>? _libroCollection;
        private readonly IMongoCollection<Usuario>? _usuarioCollection;
        private readonly IMongoCollection<VersionControlador>? _versionControladorCollection;
        private readonly IMongoCollection<Dedicatoria>? _dedicatoriaCollection;
        private readonly IMongoCollection<Glosario>? _glosarioCollection;
        private readonly ITorahJsonRepository _torahJsonRepository;

        public DataRepository(string connectionString, ITorahJsonRepository torahJsonRepository)
        {
            try
            {
                _connectionString = connectionString;

                _torahJsonRepository = torahJsonRepository;

                var mongoUrl = MongoUrl.Create(_connectionString);

                var mongoClient = new MongoClient(mongoUrl);

                _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);

                _libroCollection = _database.GetCollection<Libro>("Libro");

                _usuarioCollection = _database.GetCollection<Usuario>("Usuario");

                _versionControladorCollection = _database.GetCollection<VersionControlador>("VersionControlador");

                _dedicatoriaCollection = _database.GetCollection<Dedicatoria>("Dedicatoria");

                _glosarioCollection = _database.GetCollection<Glosario>("Glosario");
            }
            catch
            {
                throw;
            }
        }

        public async Task DataSeed()
        {
            try
            {
                var existingLibros = await _libroCollection.Find(_ => true).ToListAsync();

                if (existingLibros.Count < 1)
                {
                    var libros = await _torahJsonRepository.LoadData();

                    if (_libroCollection != null) {
                        await _libroCollection.InsertManyAsync(libros);
                    }                    
                }

                var existingUsers = await _usuarioCollection.Find(_ => true).ToListAsync();

                if (existingUsers.Count < 1)
                {
                    if (_usuarioCollection != null) {
                        await _usuarioCollection.InsertOneAsync(new Usuario
                        {
                            Email = "admin@admin.com",
                            Password = "37ca4e3aba50e2b15cd237dc58aca51f"
                        });
                    }
                }

                var existingVersionControls = await _versionControladorCollection.Find(_ => true).ToListAsync();

                if (existingVersionControls.Count < 1)
                {
                    if (_versionControladorCollection != null) {
                        await _versionControladorCollection.InsertOneAsync(new VersionControlador
                        {
                            Version = 0,
                            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                        });
                    }
                }

                var existingDedicatorias = await _dedicatoriaCollection.Find(_ => true).ToListAsync();

                if (existingDedicatorias.Count < 1)
                {
                    if (_dedicatoriaCollection != null)
                    {
                        await _dedicatoriaCollection.InsertOneAsync(new Dedicatoria { 
                            Contenido = "<h1>Dedicatoria</h1>"
                        });
                    }
                }

                var existingGlosarios = await _glosarioCollection.Find(_ => true).ToListAsync();

                if (existingGlosarios.Count < 1)
                {
                    if (_glosarioCollection != null)
                    {
                        await _glosarioCollection.InsertOneAsync(new Glosario
                        {
                            Contenido = "<h1>Glosario</h1>"
                        });
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<Usuario> GetUsuario(string? email)
        {
            try
            {
                var usuario = await _usuarioCollection.FindAsync(x => x.Email == email).Result.SingleOrDefaultAsync();
                return usuario;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<string>> ListTestamentos() 
        {
            try
            {
                var testamentos = new List<string>();

                if (_libroCollection != null) 
                {
                    var testamentoValues = await _libroCollection.DistinctAsync<string>("Testamento", FilterDefinition<Libro>.Empty);
                    testamentos = testamentoValues.ToList();
                }

                return testamentos;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Libro>> ListLibros()
        {
            try
            {
                var libros = await _libroCollection.FindAsync(_ => true);
                return libros.ToList();
            }
            catch
            {
                throw;
            }
        }

        public async Task<Libro> ListLibro(string? id)
        {
            try
            {
                var libro = await _libroCollection.FindAsync(_x => _x.Id == id);
                return libro.SingleOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdateNombreLibro(string? id, string? nombre)
        {
            try
            {
                var filter = Builders<Libro>.Filter.Eq(x => x.Id, id);

                var update = Builders<Libro>.Update.Set(x => x.Nombre, nombre);

                if (_libroCollection != null) 
                {
                    await _libroCollection.UpdateOneAsync(filter, update);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdateAbreviaturaLibro(string? id, string? abreviatura)
        {
            try
            {
                var filter = Builders<Libro>.Filter.Eq(x => x.Id, id);

                var update = Builders<Libro>.Update.Set(x => x.Abreviacion, abreviatura);

                if (_libroCollection != null) 
                {
                    await _libroCollection.UpdateOneAsync(filter, update);
                }                
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdateVersiculoLibro(string? id, int capituloNumero, int versiculoNumero, string? versiculo)
        {
            try
            {
                // Create the filter to match the document and specific capítulo
                var filter = Builders<Libro>.Filter.And(
                     Builders<Libro>.Filter.Eq(l => l.Id, id),
                     Builders<Libro>.Filter.ElemMatch(
                         l => l.Capitulos,
                         c => c.CapituloNumero == capituloNumero
                     )
                 );

                // Define the update to set the specific versículo
                var update = Builders<Libro>.Update.Set(
                    $"Capitulos.$[capitulo].Versiculos.{versiculoNumero}",
                    versiculo
                );

                // Define array filters
                var arrayFilters = new[]
                {
                    new BsonDocumentArrayFilterDefinition<BsonDocument>(
                        new BsonDocument("capitulo.CapituloNumero", capituloNumero)
                    )
                };

                // Perform the update
                var updateOptions = new UpdateOptions { ArrayFilters = arrayFilters };

                if (_libroCollection != null) 
                {
                    await _libroCollection.UpdateOneAsync(filter, update, updateOptions);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task IncremetarVersion()
        {
            try
            {
                var filter = Builders<VersionControlador>.Filter.Empty;
                var sort = Builders<VersionControlador>.Sort.Descending(doc => doc.Timestamp);

                var result = await _versionControladorCollection
                    .Find(filter)
                    .Sort(sort)
                    .Limit(1)
                    .FirstOrDefaultAsync();

                var nextVersion = result.Version + 1;

                await _versionControladorCollection.InsertOneAsync(new VersionControlador
                {
                    Version = nextVersion,
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                });
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
                var filter = Builders<VersionControlador>.Filter.Empty;
                var sort = Builders<VersionControlador>.Sort.Descending(doc => doc.Timestamp);

                var result = await _versionControladorCollection
                    .Find(filter)
                    .Sort(sort)
                    .Limit(1)
                    .FirstOrDefaultAsync();

                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Glosario>> ListGlosario() 
        {
            try 
            {
                var glosarios = await _glosarioCollection.FindAsync(_ => true);
                return glosarios.ToList();
            }
            catch 
            {
                throw;
            }
        }

        public async Task<List<Dedicatoria>> ListDedicatoria() 
        {
            try
            {
                var dedicatorias = await _dedicatoriaCollection.FindAsync(_ => true);
                return dedicatorias.ToList();
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdateGlosario(string? id, string? contenido) 
        {
            try
            {
                var filter = Builders<Glosario>.Filter.Eq(x => x.Id, id);

                var update = Builders<Glosario>.Update.Set(x => x.Contenido, contenido);

                if (_glosarioCollection != null)
                {
                    await _glosarioCollection.UpdateOneAsync(filter, update);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdateDedicatoria(string? id, string? contenido) 
        {
            try
            {
                var filter = Builders<Dedicatoria>.Filter.Eq(x => x.Id, id);

                var update = Builders<Dedicatoria>.Update.Set(x => x.Contenido, contenido);

                if (_dedicatoriaCollection != null)
                {
                    await _dedicatoriaCollection.UpdateOneAsync(filter, update);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
