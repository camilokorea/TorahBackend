namespace TorahBackend.Application.DTO
{
    // Insertar: Versiculo es el texto nuevo, que queda en la posición indicada.
    // Eliminar: Versiculo es el texto actual en esa posición, para confirmar que se borra el correcto.
    public class LibroVersiculoPosicion
    {
        public string Id { get; set; } = string.Empty;
        public int CapituloNumero { get; set; }
        public int Posicion { get; set; }
        public string Versiculo { get; set; } = string.Empty;
    }
}
