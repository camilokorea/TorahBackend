using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TorahBackend.Domain.Entities
{
    // Estudio bíblico: un tema con secciones; cada sección tiene bloques de texto, imagen o versículos.
    public class Estudio
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Resumen { get; set; } = string.Empty;

        // Imagen de portada (id en GridFS), opcional.
        public string? PortadaImagenId { get; set; }

        public bool Publicado { get; set; }

        // Posición en la lista de estudios de la app.
        public int Orden { get; set; }

        // Aumenta en cada guardado; sirve para detectar ediciones simultáneas.
        public int Revision { get; set; }

        public DateTime CreadoEn { get; set; }

        public DateTime ActualizadoEn { get; set; }

        public DateTime? PublicadoEn { get; set; }

        public List<EstudioSeccion> Secciones { get; set; } = new List<EstudioSeccion>();
    }

    public class EstudioSeccion
    {
        public string Id { get; set; } = string.Empty;

        public string Titulo { get; set; } = string.Empty;

        public List<EstudioBloque> Bloques { get; set; } = new List<EstudioBloque>();
    }

    public static class TipoBloque
    {
        public const string Texto = "texto";
        public const string Imagen = "imagen";
        public const string Versiculos = "versiculos";
    }

    public class EstudioBloque
    {
        public string Id { get; set; } = string.Empty;

        // "texto" | "imagen" | "versiculos"
        public string Tipo { get; set; } = TipoBloque.Texto;

        // Texto con formato básico (Markdown limitado: **negrita**, *cursiva*, ## subtítulo, listas, enlaces).
        public string? Texto { get; set; }

        public string? ImagenId { get; set; }

        public string? Leyenda { get; set; }

        // Para bloques de versículos: el texto no se copia, se toma de la Biblia al mostrarlo.
        public List<ReferenciaVersiculos> Referencias { get; set; } = new List<ReferenciaVersiculos>();
    }

    public class ReferenciaVersiculos
    {
        public string LibroId { get; set; } = string.Empty;

        public int Capitulo { get; set; }

        public int Desde { get; set; }

        public int Hasta { get; set; }
    }
}
