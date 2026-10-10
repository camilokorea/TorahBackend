using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using TorahBackend.Application.Interfaces;
using TorahBackend.Domain.Entities;

namespace TorahBackend.Infrastructure.Repositories
{
    public class EstudioRepository : IEstudioRepository
    {
        private readonly IMongoCollection<Estudio> _estudios;
        private readonly IMongoCollection<Libro> _libros;

        public EstudioRepository(string connectionString)
        {
            var url = MongoUrl.Create(connectionString);
            var database = new MongoClient(url).GetDatabase(url.DatabaseName);
            _estudios = database.GetCollection<Estudio>("Estudio");
            _libros = database.GetCollection<Libro>("Libro");
        }

        public async Task<List<Estudio>> Listar(bool soloPublicados)
        {
            var filtro = soloPublicados
                ? Builders<Estudio>.Filter.Eq(e => e.Publicado, true)
                : Builders<Estudio>.Filter.Empty;
            return await _estudios.Find(filtro).SortBy(e => e.Orden).ThenByDescending(e => e.CreadoEn).ToListAsync();
        }

        public async Task<Estudio?> Obtener(string id)
        {
            if (!ObjectId.TryParse(id, out _)) return null;
            return await _estudios.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public Task Insertar(Estudio estudio) => _estudios.InsertOneAsync(estudio);

        public async Task<bool> Reemplazar(Estudio estudio, int revisionEsperada)
        {
            var resultado = await _estudios.ReplaceOneAsync(e => e.Id == estudio.Id && e.Revision == revisionEsperada, estudio);
            return resultado.MatchedCount == 1;
        }

        public async Task<bool> CambiarEstado(string id, bool publicado)
        {
            if (!ObjectId.TryParse(id, out _)) return false;
            var update = Builders<Estudio>.Update
                .Set(e => e.Publicado, publicado)
                .Set(e => e.ActualizadoEn, DateTime.UtcNow)
                .Inc(e => e.Revision, 1);
            if (publicado) update = update.Set(e => e.PublicadoEn, DateTime.UtcNow);
            var resultado = await _estudios.UpdateOneAsync(e => e.Id == id, update);
            return resultado.MatchedCount == 1;
        }

        public async Task<bool> Eliminar(string id)
        {
            if (!ObjectId.TryParse(id, out _)) return false;
            var resultado = await _estudios.DeleteOneAsync(e => e.Id == id);
            return resultado.DeletedCount == 1;
        }

        public async Task ActualizarOrden(IReadOnlyList<string> ids)
        {
            var operaciones = ids
                .Select((id, i) => (id, i))
                .Where(x => ObjectId.TryParse(x.id, out _))
                .Select(x => new UpdateOneModel<Estudio>(
                    Builders<Estudio>.Filter.Eq(e => e.Id, x.id),
                    Builders<Estudio>.Update.Set(e => e.Orden, x.i)))
                .ToList();
            if (operaciones.Count > 0) await _estudios.BulkWriteAsync(operaciones);
        }

        public async Task<bool> LibroExiste(string libroId)
        {
            if (!ObjectId.TryParse(libroId, out _)) return false;
            return await _libros.Find(l => l.Id == libroId).AnyAsync();
        }
    }

    // Imágenes de los estudios guardadas en MongoDB (GridFS, bucket "imagenes").
    public class ImagenRepository : IImagenRepository
    {
        private readonly GridFSBucket _bucket;

        public ImagenRepository(string connectionString)
        {
            var url = MongoUrl.Create(connectionString);
            var database = new MongoClient(url).GetDatabase(url.DatabaseName);
            _bucket = new GridFSBucket(database, new GridFSBucketOptions { BucketName = "imagenes" });
        }

        public async Task<string> Subir(Stream contenido, string nombre, string tipoContenido)
        {
            var opciones = new GridFSUploadOptions
            {
                Metadata = new BsonDocument { { "tipoContenido", tipoContenido }, { "subidoEn", DateTime.UtcNow } }
            };
            var id = await _bucket.UploadFromStreamAsync(nombre, contenido, opciones);
            return id.ToString();
        }

        public async Task<(byte[] Datos, string TipoContenido)?> Obtener(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return null;
            var info = await (await _bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Eq("_id", objectId))).FirstOrDefaultAsync();
            if (info == null) return null;
            var datos = await _bucket.DownloadAsBytesAsync(objectId);
            var tipo = info.Metadata != null && info.Metadata.Contains("tipoContenido")
                ? info.Metadata["tipoContenido"].AsString
                : "application/octet-stream";
            return (datos, tipo);
        }

        public async Task Eliminar(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return;
            try
            {
                await _bucket.DeleteAsync(objectId);
            }
            catch (GridFSFileNotFoundException)
            {
                // Ya no existía.
            }
        }
    }
}
