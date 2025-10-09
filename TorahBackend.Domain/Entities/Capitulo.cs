using System.Text.Json.Serialization;

namespace TorahBackend.Domain.Entities
{
    public class Capitulo
    {
        [JsonPropertyName("capituloNumero")]
        public int CapituloNumero { get; set; }

        [JsonPropertyName("versiculos")]
        public List<string> Versiculos { get; set; } = new List<string>();
    }
}
