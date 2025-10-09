namespace TorahBackend.Application.DTO
{
    public class LibroVersiculo
    {
        public string Id { get; set; } = string.Empty;
        public int CapituloNumero { get; set; }
        public int VersiculoNumero { get; set; }
        public string Versiculo { get; set; } = string.Empty;
    }
}
