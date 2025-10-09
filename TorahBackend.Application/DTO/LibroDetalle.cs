namespace TorahBackend.Application.DTO
{
    public class LibroDetalle
    {
        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public string? Abreviacion { get; set; }
        public string? Testamento { get; set; }
        public List<CapituloDetalle> Capitulos { get; set; } = new List<CapituloDetalle>();
    }
}
