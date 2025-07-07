using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Npgsql.PostgresTypes;
using IterumApi.DTOs;

namespace IterumApi.Models
{
    public class Map
    {
        public Map() { }

        public Map(MapDto mapDto, long userId) {
            if (mapDto.Id != null) {
                Id = mapDto.Id;
            }
            Name = mapDto.Name;
            UserId = userId;
            Hexes = mapDto.Hexes;
            IsFlatTopped = mapDto.IsFlatTopped;
            MaxX = mapDto.maxX;
            MaxY = mapDto.maxY;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; } = null!;
        public List<Hex> Hexes { get; set; } = [];
        public bool IsFlatTopped { get; set; }
        public int MaxX { get; set; }
        public int MaxY { get; set; }
    }
}
