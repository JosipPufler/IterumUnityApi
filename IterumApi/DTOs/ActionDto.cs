namespace IterumApi.DTOs
{
    public class ActionDto
    {
        public ActionDto(){}

        public ActionDto(string id, string name, string description, int apCost, int mpCost, string data)
        {
            Id = id;
            Name = name;
            Description = description;
            ApCost = apCost;
            MpCost = mpCost;
            Data = data;
        }

        public string? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ApCost { get; set; }
        public int MpCost { get; set; }
        public string Data { get; set; }
    }
}
