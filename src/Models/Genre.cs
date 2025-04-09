using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace videogame_api.src.Models
{
    public class Genre
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [JsonIgnore]
        public List<VideogameInstance> VideogameInstances { get; set; } = [];
        [ConcurrencyCheck]
        public DateTime Version { get; set; } = DateTime.Now;
    }
}
