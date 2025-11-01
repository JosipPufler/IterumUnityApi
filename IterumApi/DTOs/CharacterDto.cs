namespace IterumApi.DTOs
{
    public class CharacterDto
    {
        public CharacterDto(string? id, string name, int level, bool isPlayer, string data, long ownerId)
        {
            Id = id;
            Name = name;
            Level = level;
            IsPlayer = isPlayer;
            Data = data;
            OwnerId = ownerId;
        }

        public string? Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public bool IsPlayer { get; set; }
        public string Data { get; set; }
        public long OwnerId { get; set; }
    }
}
