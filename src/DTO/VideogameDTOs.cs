using videogame_api.src.Models;

namespace videogame_api.src.DTO
{
    // DTOs
    public record VideogamePostPutDTO(string Name, string Description, string Platform, List<string> Genres)
    {
        public required string Name { get; set; } = Name;
        public required string Description { get; set; } = Description;
        public required string Platform { get; set; } = Platform;
        public required List<string> Genres { get; set; } = Genres;
    }
    public record VideogamePublishableDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Platform { get; set; } = null!;
        public List<string> Genres { get; set; } = null!;
    }
}
