namespace TorahBackend.Application.DTO
{
    public class CapituloDetalle
    {
        public int CapituloNumero { get; set; }
        public List<string> Versiculos { get; set; } = new List<string>();
    }
}
