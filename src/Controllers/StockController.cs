using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using videogame_api.src.Models;
using videogame_api.src.DTO;
using Microsoft.AspNetCore.Authorization;

namespace videogame_api.src.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class StockController(AppDbContext context) : ControllerBase
    {
        // member fields
        private readonly AppDbContext _context = context;

        // action methods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockPublishableDTO>>> GetStocks()
        {
            return await _context.StockSet
                .Include(it => it.Videogame)
                .ThenInclude(it => it.Genres)
                .Include(it => it.Videogame)
                .ThenInclude(it => it.Platforms)
                .Select(it => ToPublishableDTO(it))
                .ToListAsync();
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<StockPublishableDTO>> GetStock(int id)
        {
            var stock = await _context.StockSet
                .Include(it => it.Videogame)
                .ThenInclude(it => it.Genres)
                .Include(it => it.Videogame)
                .ThenInclude(it => it.Platforms)
                .FirstOrDefaultAsync(it => it.Id == id);

            if (stock == null)
                return NotFound();

            return ToPublishableDTO(stock);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<StockPublishableDTO>> PostStock(StockPostDTO stock)
        {
            var videogameInstance = await _context.VideogamesSet
                .Include(it => it.Genres)
                .Include(it => it.Platforms)
                .SingleOrDefaultAsync(it => it.Id == stock.VideogameId);
            if (videogameInstance == null)
                return NotFound();

            Stock stockInstance = new()
            {
                VideogameId = stock.VideogameId,
                Videogame = videogameInstance,
                Amount = stock.Amount,
            };

            _context.StockSet.Add(stockInstance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStock), new { id = stockInstance.Id }, ToPublishableDTO(stockInstance));
        }

        [HttpPatch("json-patch-doc/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> JsonPatchDocStock([FromRoute] int id, JsonPatchDocument<Stock> patchDocument)
        {
            var stockInstance = await _context.StockSet.FirstOrDefaultAsync(it => it.Id == id);

            if (stockInstance == null)
                return NotFound();
            
            if (patchDocument == null)
                return BadRequest();

            patchDocument.ApplyTo(stockInstance);

            stockInstance.Version = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteStock(int id)
        {
            var stock = await _context.StockSet.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            _context.StockSet.Remove(stock);
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
        private static StockPublishableDTO ToPublishableDTO(Stock stock)
        {
            return new StockPublishableDTO {
                Id = stock.Id,
                VideogameId = stock.VideogameId,
                Videogame = ToVideogamePublishableDTO(stock.Videogame),
                Amount = stock.Amount,
            };
        }
    }
}
