using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace game_freak_pokemon_api.Models
{
    public class Ability
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public string? Name { get; set; }

        [BsonElement("description"), BsonRepresentation(BsonType.String)]
        public string? Description { get; set; }
        
        [BsonElement("damage"), BsonRepresentation(BsonType.Int32)]
        public int? Damage { get; set; }

        [BsonElement("type"), BsonRepresentation(BsonType.String)]
        public string? Type { get; set; }
    }
}