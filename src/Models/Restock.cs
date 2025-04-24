using System.ComponentModel.DataAnnotations;

namespace videogame_api.src.Models
{
    public class Restock
    {
        public int Id { get; set; }
        [Required]
        public int StockId { get; set; } = 0;
        [Required]
        public Stock Stock { get; set; } = null!;
        [Required]
        public int RestockAmount { get; set; } = 0;
        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
