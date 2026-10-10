using MongoDB.Bson;
using TorahBackend.Application.DTO;
using TorahBackend.Application.Interfaces;
using TorahBackend.Domain.Entities;

namespace TorahBackend.Application.Services
{
    public interface IEstudioService
    {
        Task<List<EstudioResumen>> ListarTodos();
        Task<List<Estudio>> ListarPublicados();
        Task<Estudio?> Obtener(string id);
        Task<Estudio> Crear(string titulo);
        Task<EstudioResultado> Guardar(EstudioGuardar datos);
        Task<bool> CambiarEstado(string id, bool publicado);
        Task<bool> Eliminar(string id);
        Task Ordenar(IReadOnlyList<string> ids);
    }

    public class EstudioService : IEstudioService
    {
        private const int MaxTitulo = 200;
        private const int MaxResumen = 1000;
        private const int MaxTexto = 30000;
        private const int MaxSecciones = 100;
        private const int MaxBloques = 200;

        private static readonly HashSet<string> Tipos = new() { TipoBloque.Texto, TipoBloque.Imagen, TipoBloque.Versiculos };

        private readonly IEstudioRepository _repositorio;
        private readonly IImagenRepository _imagenes;

        public EstudioService(IEstudioRepository repositorio, IImagenRepository imagenes)
        {
            _repositorio = repositorio;
            _imagenes = imagenes;
        }

        public async Task<List<EstudioResumen>> ListarTodos()
        {
            var estudios = await _repositorio.Listar(false);
            return estudios.Select(e => new EstudioResumen
            {
                Id = e.Id,
                Titulo = e.Titulo,
                Resumen = e.Resumen,
                PortadaImagenId = e.PortadaImagenId,
                Publicado = e.Publicado,
                Orden = e.Orden,
                Secciones = e.Secciones.Count,
                ActualizadoEn = e.ActualizadoEn,
                PublicadoEn = e.PublicadoEn,
            }).ToList();
        }

        public Task<List<Estudio>> ListarPublicados() => _repositorio.Listar(true);

        public Task<Estudio?> Obtener(string id) => _repositorio.Obtener(id);

        public async Task<Estudio> Crear(string titulo)
        {
            var existentes = await _repositorio.Listar(false);
            var ahora = DateTime.UtcNow;
            var estudio = new Estudio
            {
                Titulo = Limitar(titulo, MaxTitulo),
                Orden = existentes.Count == 0 ? 0 : existentes.Max(e => e.Orden) + 1,
                CreadoEn = ahora,
                ActualizadoEn = ahora,
                Secciones = new List<EstudioSeccion>
                {
                    new EstudioSeccion { Id = NuevoId(), Titulo = "Introducción" }
                }
            };
            await _repositorio.Insertar(estudio);
            return estudio;
        }

        public async Task<EstudioResultado> Guardar(EstudioGuardar datos)
        {
            var actual = await _repositorio.Obtener(datos.Id);
            if (actual == null) return new EstudioResultado(ResultadoOperacion.NoEncontrado);
            if (actual.Revision != datos.Revision)
                return new EstudioResultado(ResultadoOperacion.Conflicto, Mensaje: "Otra persona guardó cambios en este estudio. Recarga para ver la versión actual.");

            var error = await Validar(datos);
            if (error != null) return new EstudioResultado(ResultadoOperacion.Invalido, Mensaje: error);

            var nuevo = new Estudio
            {
                Id = actual.Id,
                Titulo = datos.Titulo.Trim(),
                Resumen = (datos.Resumen ?? string.Empty).Trim(),
                PortadaImagenId = string.IsNullOrWhiteSpace(datos.PortadaImagenId) ? null : datos.PortadaImagenId,
                Publicado = actual.Publicado,
                Orden = actual.Orden,
                Revision = actual.Revision + 1,
                CreadoEn = actual.CreadoEn,
                ActualizadoEn = DateTime.UtcNow,
                PublicadoEn = actual.PublicadoEn,
                Secciones = datos.Secciones.Select(s => new EstudioSeccion
                {
                    Id = string.IsNullOrWhiteSpace(s.Id) ? NuevoId() : s.Id,
                    Titulo = (s.Titulo ?? string.Empty).Trim(),
                    Bloques = s.Bloques.Select(Normalizar).ToList(),
                }).ToList(),
            };

            if (!await _repositorio.Reemplazar(nuevo, actual.Revision))
                return new EstudioResultado(ResultadoOperacion.Conflicto, Mensaje: "Otra persona guardó cambios en este estudio. Recarga para ver la versión actual.");

            await BorrarImagenesHuerfanas(ImagenesDe(actual).Except(ImagenesDe(nuevo)), nuevo.Id);
            return new EstudioResultado(ResultadoOperacion.Ok, nuevo);
        }

        public Task<bool> CambiarEstado(string id, bool publicado) => _repositorio.CambiarEstado(id, publicado);

        public async Task<bool> Eliminar(string id)
        {
            var estudio = await _repositorio.Obtener(id);
            if (estudio == null || !await _repositorio.Eliminar(id)) return false;
            await BorrarImagenesHuerfanas(ImagenesDe(estudio), id);
            return true;
        }

        public Task Ordenar(IReadOnlyList<string> ids) => _repositorio.ActualizarOrden(ids);

        private async Task<string?> Validar(EstudioGuardar datos)
        {
            if (string.IsNullOrWhiteSpace(datos.Titulo)) return "El estudio necesita un título.";
            if (datos.Titulo.Length > MaxTitulo) return $"El título no puede pasar de {MaxTitulo} caracteres.";
            if ((datos.Resumen ?? string.Empty).Length > MaxResumen) return $"El resumen no puede pasar de {MaxResumen} caracteres.";
            if (!EsIdValido(datos.PortadaImagenId, opcional: true)) return "La imagen de portada no es válida.";
            if (datos.Secciones == null || datos.Secciones.Count > MaxSecciones) return $"Un estudio puede tener hasta {MaxSecciones} secciones.";

            var libros = new Dictionary<string, bool>();
            for (var i = 0; i < datos.Secciones.Count; i++)
            {
                var seccion = datos.Secciones[i];
                var donde = $"Sección {i + 1}";
                if ((seccion.Titulo ?? string.Empty).Length > MaxTitulo) return $"{donde}: el título es demasiado largo.";
                if (seccion.Bloques == null || seccion.Bloques.Count > MaxBloques) return $"{donde}: demasiados bloques.";

                for (var j = 0; j < seccion.Bloques.Count; j++)
                {
                    var bloque = seccion.Bloques[j];
                    var lugar = $"{donde}, bloque {j + 1}";
                    if (!Tipos.Contains(bloque.Tipo)) return $"{lugar}: tipo de bloque desconocido.";

                    if (bloque.Tipo == TipoBloque.Texto && (bloque.Texto ?? string.Empty).Length > MaxTexto)
                        return $"{lugar}: el texto no puede pasar de {MaxTexto} caracteres.";

                    if (bloque.Tipo == TipoBloque.Imagen && !EsIdValido(bloque.ImagenId, opcional: false))
                        return $"{lugar}: falta la imagen.";

                    if (bloque.Tipo == TipoBloque.Versiculos)
                    {
                        if (bloque.Referencias == null || bloque.Referencias.Count == 0) return $"{lugar}: agrega al menos una referencia.";
                        foreach (var r in bloque.Referencias)
                        {
                            if (r.Capitulo < 1 || r.Desde < 1 || r.Hasta < r.Desde) return $"{lugar}: la referencia no es válida.";
                            if (!libros.TryGetValue(r.LibroId, out var existe))
                            {
                                existe = await _repositorio.LibroExiste(r.LibroId);
                                libros[r.LibroId] = existe;
                            }
                            if (!existe) return $"{lugar}: la referencia apunta a un libro que no existe.";
                        }
                    }
                }
            }
            return null;
        }

        // Deja en cada bloque solo los campos de su tipo.
        private static EstudioBloque Normalizar(EstudioBloque b) => new EstudioBloque
        {
            Id = string.IsNullOrWhiteSpace(b.Id) ? NuevoId() : b.Id,
            Tipo = b.Tipo,
            Texto = b.Tipo == TipoBloque.Texto ? (b.Texto ?? string.Empty).Trim() : null,
            ImagenId = b.Tipo == TipoBloque.Imagen ? b.ImagenId : null,
            Leyenda = b.Tipo == TipoBloque.Imagen ? (b.Leyenda ?? string.Empty).Trim() : null,
            Referencias = b.Tipo == TipoBloque.Versiculos ? b.Referencias : new List<ReferenciaVersiculos>(),
        };

        private static IEnumerable<string> ImagenesDe(Estudio e) =>
            e.Secciones.SelectMany(s => s.Bloques).Select(b => b.ImagenId)
                .Append(e.PortadaImagenId)
                .Where(id => !string.IsNullOrEmpty(id))
                .Select(id => id!)
                .Distinct();

        // Borra imágenes que ya no usa este estudio, salvo que otro estudio las use.
        private async Task BorrarImagenesHuerfanas(IEnumerable<string> candidatas, string? estudioId)
        {
            var lista = candidatas.ToList();
            if (lista.Count == 0) return;
            var enUso = (await _repositorio.Listar(false))
                .Where(e => e.Id != estudioId)
                .SelectMany(ImagenesDe)
                .ToHashSet();
            foreach (var id in lista.Where(id => !enUso.Contains(id)))
            {
                await _imagenes.Eliminar(id);
            }
        }

        private static bool EsIdValido(string? id, bool opcional) =>
            string.IsNullOrWhiteSpace(id) ? opcional : ObjectId.TryParse(id, out _);

        private static string NuevoId() => Guid.NewGuid().ToString("N")[..12];

        private static string Limitar(string? texto, int max)
        {
            var t = (texto ?? string.Empty).Trim();
            if (t.Length == 0) t = "Nuevo estudio";
            return t.Length > max ? t[..max] : t;
        }
    }
}
