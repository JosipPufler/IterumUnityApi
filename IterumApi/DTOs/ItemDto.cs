using IterumApi.Models;

namespace IterumApi.DTOs
{
    public class ItemDto
    {
        public ItemDto(){}

        public ItemDto(string? id, string name, ItemType type, string description, string data)
        {
            Id = id;
            Name = name;
            Type = type;
            Data = data;
            Description = description;
        }

        public ItemDto(Item item) {
            Id = item.Id;
            Name = item.Name;
            Type = item.Type;
            Data = item.Data;
            Description = item.Description;
        }

        public string? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; set; }
        public string Data { get; set; }
    }
}
