namespace videogame_api.src.DTO
{
    // DTOs
    public record PlatformPostPutDTO(string Name)
    {
        public required string Name { get; set; } = Name;
    }
    public record PlatformPublishableDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
