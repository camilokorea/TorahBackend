using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace TorahBackend.Domain.Entities
{
    public class Dedicatoria
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [JsonPropertyName("contenido")]
        public string? Contenido { get; set; }
    }
}
