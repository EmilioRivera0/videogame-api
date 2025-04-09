using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using videogame_api.src.DTO;
using videogame_api.src.Models;

namespace videogame_api.src.Controllers
{
    [Route("api/[controller]")]
    public class VideogamesController(AppDbContext context) : ControllerBase
    {
        // member fields
        private readonly AppDbContext _context = context;

        // action methods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VideogamePublishableDTO>>> GetVideogameInstances()
        {
            return await _context.VideogamesSet.Include(it => it.Genres).Select(it => ToPublishableDTO(it)).ToListAsync();
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<VideogamePublishableDTO>> GetVideogameInstance(int id)
        {
            var videogameInstance = await _context.VideogamesSet.Include(it => it.Genres).FirstOrDefaultAsync(it => it.Id == id);

            if (videogameInstance == null)
                return NotFound();

            return ToPublishableDTO(videogameInstance);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PostVideogameInstance(VideogamePostPutDTO videogameDTO)
        {
            var videogameInstance = await ToVideogameInstance(videogameDTO);
            _context.VideogamesSet.Add(videogameInstance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVideogameInstance), new { id = videogameInstance.Id }, ToPublishableDTO(videogameInstance));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PutVideogameInstance(int id, VideogamePostPutDTO videogameDTO)
        {
            var videogameInstance = await _context.VideogamesSet.Include(it => it.Genres).FirstOrDefaultAsync(it => it.Id == id);
            
            if (videogameInstance == null)
                return NotFound();
            
            videogameInstance.Name = videogameDTO.Name;
            videogameInstance.Description = videogameDTO.Description;
            videogameInstance.Platform = videogameDTO.Platform;
            videogameInstance.Genres.Clear();
            videogameInstance.Genres = videogameDTO.Genres.Select(obj =>
            {
                var genre = _context.GenresSet.FirstOrDefault(it => it.Name == obj);
                if (genre == null)
                    genre = new Genre { Name = obj };
                return genre;
            }).ToList();

            videogameInstance.Version = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VideogameInstanceExists(id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PatchVideogameInstance([FromRoute] int id, JsonPatchDocument<VideogameInstance> patchDocument)
        {
            var videogameInstance = await _context.VideogamesSet.Include(it => it.Genres).FirstOrDefaultAsync(it => it.Id == id);

            if (videogameInstance == null)
                return NotFound();

            if (patchDocument == null)
                return BadRequest();

            patchDocument.ApplyTo(videogameInstance, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            videogameInstance.Version = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteVideogameInstance(int id)
        {
            var videogameInstance = await _context.VideogamesSet.FindAsync(id);
            if (videogameInstance == null)
                return NotFound();

            _context.VideogamesSet.Remove(videogameInstance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // member methods
        private bool VideogameInstanceExists(int id) => _context.VideogamesSet.Any(e => e.Id == id);
        private static VideogamePublishableDTO ToPublishableDTO(VideogameInstance videogameInstance)
        {
            return new VideogamePublishableDTO
            {
                Id = videogameInstance.Id,
                Name = videogameInstance.Name,
                Description = videogameInstance.Description,
                Platform = videogameInstance.Platform,
                Genres = [.. videogameInstance.Genres.Select(it => it.Name)]
            };
        }
        private async Task<VideogameInstance> ToVideogameInstance(VideogamePostPutDTO videogamePostPutDTO)
        {
            Genre temp;
            List<Genre> genres = [];

            foreach (string genre in videogamePostPutDTO.Genres)
            {
                temp = await _context.GenresSet.FirstOrDefaultAsync(it => it.Name == genre);

                if (temp == null)
                    temp = new Genre{Name = genre};

                genres.Add(temp);
            }

            return new VideogameInstance
            {
                Name = videogamePostPutDTO.Name,
                Description = videogamePostPutDTO.Description,
                Platform = videogamePostPutDTO.Platform,
                Genres = genres
            };
        }
    }
}
