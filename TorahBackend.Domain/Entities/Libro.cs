using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;


namespace TorahBackend.Domain.Entities
{
    public class Libro
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; }

        [JsonPropertyName("abreviacion")]
        public string? Abreviacion { get; set; }

        [JsonPropertyName("testamento")]
        public string? Testamento { get; set; }

        [JsonPropertyName("capitulos")]
        public List<Capitulo> Capitulos { get; set; } = new List<Capitulo>();
    }
}
