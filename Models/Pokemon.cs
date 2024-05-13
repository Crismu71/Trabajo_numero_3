using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace game_freak_pokemon_api.Models
{
    public class Pokemon
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public string? Name { get; set; }

        [BsonElement("type"), BsonRepresentation(BsonType.String)]
        public string? Type { get; set; }

        [BsonElement("hit_points"), BsonRepresentation(BsonType.Int32)]
        public int? HitPoints { get; set; }

        [BsonElement("defense"), BsonRepresentation(BsonType.Decimal128)]
        public decimal? Defense { get; set; }

        [BsonElement("abilities")]
        public List<Ability>? Abilities { get; set; }
    }
}
