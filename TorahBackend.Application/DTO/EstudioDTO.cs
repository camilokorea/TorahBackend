using TorahBackend.Domain.Entities;

namespace TorahBackend.Application.DTO
{
    public class EstudioResumen
    {
        public string? Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Resumen { get; set; } = string.Empty;
        public string? PortadaImagenId { get; set; }
        public bool Publicado { get; set; }
        public int Orden { get; set; }
        public int Secciones { get; set; }
        public DateTime ActualizadoEn { get; set; }
        public DateTime? PublicadoEn { get; set; }
    }

    public class EstudioCrear
    {
        public string Titulo { get; set; } = string.Empty;
    }

    // Guardado completo del estudio. Revision debe ser la que se leyó; si cambió, se responde 409.
    public class EstudioGuardar
    {
        public string Id { get; set; } = string.Empty;
        public int Revision { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Resumen { get; set; } = string.Empty;
        public string? PortadaImagenId { get; set; }
        public List<EstudioSeccion> Secciones { get; set; } = new List<EstudioSeccion>();
    }

    public class EstudioEstado
    {
        public string Id { get; set; } = string.Empty;
        public bool Publicado { get; set; }
    }

    public class EstudioId
    {
        public string Id { get; set; } = string.Empty;
    }

    public class EstudiosOrden
    {
        public List<string> Ids { get; set; } = new List<string>();
    }

    public record EstudioResultado(ResultadoOperacion Resultado, Estudio? Estudio = null, string? Mensaje = null);
}
