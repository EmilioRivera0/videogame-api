namespace videogame_api.src.DTO
{
    // DTOs
    public record GenrePostPutDTO(string Name)
    {
        public required string Name { get; set; } = Name;
    }
    public record GenrePublishableDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
