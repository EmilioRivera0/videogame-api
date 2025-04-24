namespace videogame_api.src.DTO
{
    // DTOs
    public record StockPostDTO(int VideogameId, int Amount)
    {
        public required int VideogameId { get; set; } = VideogameId;
        public required int Amount { get; set; } = Amount;
    }
    public record StockPatchDTO(int Amount)
    {
        public required int Amount { get; set; } = Amount;
    }
    public record StockPublishableDTO
    {
        public int Id { get; set; }
        public int VideogameId { get; set; } = 0;
        public VideogamePublishableDTO Videogame { get; set; } = null!;
        public int Amount { get; set; } = 0;
    }
}
