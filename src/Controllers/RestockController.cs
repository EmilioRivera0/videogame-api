using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using videogame_api.src.DTO;
using videogame_api.src.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace videogame_api.src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RestockController(AppDbContext context) : ControllerBase
    {
        // member fields
        private readonly AppDbContext _context = context;

        // action methods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RestockPublishableDTO>>> GetRestockSet()
        {
            return await _context.RestockSet
                .Include(it => it.Stock)
                .ThenInclude(it => it.Videogame)
                .ThenInclude(it => it.Genres)
                .Include(it => it.Stock)
                .ThenInclude(it => it.Videogame)
                .ThenInclude(it => it.Platforms)
                .Select(it => ToPublishableDTO(it))
                .ToListAsync();
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<RestockPublishableDTO>> GetRestock(int id)
        {
            var restock = await _context.RestockSet
                .Include(it => it.Stock)
                .ThenInclude(it => it.Videogame)
                .ThenInclude(it => it.Genres)
                .Include(it => it.Stock)
                .ThenInclude(it => it.Videogame)
                .ThenInclude(it => it.Platforms)
                .SingleOrDefaultAsync(it => it.Id == id);

            if (restock == null)
                return NotFound();

            return ToPublishableDTO(restock);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<RestockPublishableDTO>> PostRestock(RestockPostDTO restock)
        {
            var stockInstance = await _context.StockSet
                .Include (it => it.Videogame)
                .ThenInclude( it => it.Genres)
                .Include (it => it.Videogame)
                .ThenInclude (it => it.Platforms)
                .SingleOrDefaultAsync(it => it.Id == restock.StockId);
            if (stockInstance == null)
                return NotFound();

            // update stock
            stockInstance.Amount += restock.RestockAmount;
            stockInstance.Version = DateTime.Now;

            Restock restockInstance = new()
            {
                StockId = restock.StockId,
                Stock = stockInstance,
                RestockAmount = restock.RestockAmount,
            };

            _context.RestockSet.Add(restockInstance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRestock), new { id = restockInstance.Id }, ToPublishableDTO(restockInstance));
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteRestock(int id)
        {
            var restock = await _context.RestockSet.FindAsync(id);
            if (restock == null)
                return NotFound();

            _context.RestockSet.Remove(restock);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // member methods
        private static VideogamePublishableDTO ToVideogamePublishableDTO(VideogameInstance videogameInstance)
        {
            return new VideogamePublishableDTO
            {
                Id = videogameInstance.Id,
                Name = videogameInstance.Name,
                Description = videogameInstance.Description,
                Platforms = [.. videogameInstance.Platforms.Select(it => it.Name)],
                Genres = [.. videogameInstance.Genres.Select(it => it.Name)]
            };
        }
        private static StockPublishableDTO ToStockPublishableDTO(Stock stock)
        {
            return new StockPublishableDTO
            {
                Id = stock.Id,
                VideogameId = stock.VideogameId,
                Videogame = ToVideogamePublishableDTO(stock.Videogame),
                Amount = stock.Amount,
            };
        }
        private static RestockPublishableDTO ToPublishableDTO(Restock restock)
        {
            return new RestockPublishableDTO {
                Id = restock.Id,
                StockId = restock.StockId,
                Stock = ToStockPublishableDTO(restock.Stock),
                RestockAmount = restock.RestockAmount,
                Date = restock.Date,
            };
        }
    }
}
