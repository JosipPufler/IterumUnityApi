using IterumApi.Models;

namespace IterumApi.DTOs
{
    public class MapDto
    {
        public string? Id { get; set; }
        public string Name {  get; set; }
        public List<Hex> Hexes { get; set; }
        public bool IsFlatTopped { get; set; }
        public int MaxX { get; set; }
        public int MaxY { get; set; }

        public MapDto()
        {
            
        }

        public MapDto(string id, string name, List<Hex> hexs, bool isFlatTopped, int maxX, int maxY)
        {
            Name = name;
            Hexes = hexs;
            Id = id;
            IsFlatTopped = isFlatTopped;
            MaxX = maxX;
            MaxY = maxY;
        }
    }
}
