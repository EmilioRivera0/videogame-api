using System.ComponentModel.DataAnnotations;

namespace videogame_api.src.Models
{
    public class VideogameInstance
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        public List<Platform> Platforms { get; set; } = null!;
        public List<Genre> Genres { get; set; } = [];
        [ConcurrencyCheck]
        public DateTime Version { get; set; } = DateTime.Now;
    }
}
