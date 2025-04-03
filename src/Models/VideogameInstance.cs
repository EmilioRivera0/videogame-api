namespace videogame_api.src.Models
{
    public class VideogameInstance
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Platform { get; set; }
    }
}
