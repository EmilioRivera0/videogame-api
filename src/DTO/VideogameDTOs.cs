namespace videogame_api.src.DTO
{
    // DTOs
    public record VideogamePostPutDTO(string Name, string Description, string Platform)
    {
        public required string Name { get; set; } = Name;
        public required string Description { get; set; } = Description;
        public required string Platform { get; set; } = Platform;
    }
    public record VideogamePublishableDTO
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Platform { get; set; }
    }
}
