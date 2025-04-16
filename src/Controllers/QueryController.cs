using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using videogame_api.src.Models;
using videogame_api.src.DTO;

namespace videogame_api.src.Controllers
{
    [Route("api/[controller]")]
    public class QueryController(AppDbContext context) : ControllerBase
    {
        // member fields
        private readonly AppDbContext _context = context;

        // action methods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VideogamePublishableDTO>>> QueryVideogames([FromQuery] string genre = null!, [FromQuery] string name = null!)
        {
            IQueryable<VideogameInstance> query = _context.VideogamesSet.Include(it => it.Genres);

            if (genre != null)
                query = query.Where(it => it.Genres.Any(obj => obj.Name.Contains(genre, StringComparison.CurrentCultureIgnoreCase)));
            if (name != null)
                query = query.Where(it => it.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase));

            return await query.Select(it => new VideogamePublishableDTO
            {
                Name = it.Name,
                Description = it.Description,
                Platform = it.Platform,
                Genres = it.Genres.Select(obj => obj.Name).ToList()
            }).ToListAsync();
        }
    }
}
