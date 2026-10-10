namespace TorahBackend.Domain.Entities
{
    public enum ResultadoOperacion
    {
        Ok,
        NoEncontrado,
        PosicionInvalida,
        // El capítulo cambió desde que se leyó (otra edición en paralelo).
        Conflicto
    }
}
