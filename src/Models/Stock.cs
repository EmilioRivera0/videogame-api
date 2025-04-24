using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace videogame_api.src.Models
{
    public class Stock
    {
        public int Id { get; set; }
        [Required]
        public int VideogameId { get; set; }
        [Required]
        public VideogameInstance Videogame { get; set; } = null!;
        [Required]
        public int Amount { get; set; } = 0;
        [JsonIgnore]
        public List<Restock> Restocks { get; set; } = [];
        [ConcurrencyCheck]
        public DateTime Version { get; set; } = DateTime.Now;
    }
}
