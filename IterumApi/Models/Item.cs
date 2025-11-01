using IterumApi.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IterumApi.Models
{
    public class Item
    {
        public Item(){}

        public Item(string id, long userId, string name, ItemType type, string description, string data)
        {
            Id = id;
            UserId = userId;
            Name = name;
            Description = description;
            Type = type;
            Data = data;
        }

        public Item(ItemDto itemDto, long userId) {
            if (itemDto.Id != null){
                Id = itemDto.Id;
            }
            UserId = userId; 
            Name = itemDto.Name;
            Description = itemDto.Description;
            Type = itemDto.Type; 
            Data = itemDto.Data;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; set; }
        public string Data { get; set; }
    }
}
