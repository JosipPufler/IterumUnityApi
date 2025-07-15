using IterumApi.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IterumApi.Models
{
    public class Action
    {
        public Action(ActionDto action, long userId)
        {
            if (action.Id != null)
            {
                Id = action.Id;
            }
            Name = action.Name;
            UserId = userId;
            Description = action.Description;
            ApCost = action.ApCost;
            MpCost = action.MpCost;
            Data = action.Data;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ApCost { get; set; }
        public int MpCost { get; set; }
        public string Data { get; set; }
    }
}
