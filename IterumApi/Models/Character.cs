using IterumApi.DTOs;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace IterumApi.Models
{
    public class Character
    {
        public Character(CharacterDto characterDto)
        {
            if (characterDto.Id != null)
            {
                Id = characterDto.Id;
            }
            Name = characterDto.Name;
            UserId = characterDto.OwnerId;
            Level = characterDto.Level;
            IsPlayer = characterDto.IsPlayer;
            Data = characterDto.Data;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public long UserId { get; set; }
        public int Level { get; set; }
        public bool IsPlayer { get; set; }
        public string Data { get; set; }
    }
}
