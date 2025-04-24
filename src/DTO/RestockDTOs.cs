namespace videogame_api.src.DTO
{
    // DTOs
    public record RestockPostDTO(int VideogameId, int Amount)
    {
        public required int StockId { get; set; } = VideogameId;
        public required int RestockAmount { get; set; } = Amount;
    }
    public record RestockPublishableDTO
    {
        public required int Id { get; set; } = 0;
        public int StockId { get; set; } = 0;
        public StockPublishableDTO Stock  { get; set; } = null!;
        public required int RestockAmount { get; set; } = 0;
        public required DateTime Date {  get; set; }
    }
}
