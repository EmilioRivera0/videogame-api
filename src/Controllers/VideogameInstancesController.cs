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
            return await _context.VideogamesSet
                .Include(it => it.Platforms)
                .Include(it => it.Genres)
                .Select(it => ToPublishableDTO(it)).ToListAsync();
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<VideogamePublishableDTO>> GetVideogameInstance(int id)
        {
            var videogameInstance = await _context.VideogamesSet
                .Include(it => it.Platforms)
                .Include(it => it.Genres)
                .FirstOrDefaultAsync(it => it.Id == id);

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
            var videogameInstance = await _context.VideogamesSet
                .Include(it => it.Platforms)
                .Include(it => it.Genres)
                .FirstOrDefaultAsync(it => it.Id == id);
            
            if (videogameInstance == null)
                return NotFound();
            
            videogameInstance.Name = videogameDTO.Name;
            videogameInstance.Description = videogameDTO.Description;
            videogameInstance.Platforms.Clear();
            videogameInstance.Platforms = [.. videogameDTO.Platforms.Select(obj =>
            {
                var platform = _context.PlatformSet.FirstOrDefault(it => it.Name == obj);
                if (platform == null)
                    platform = new Platform { Name = obj };
                return platform;
            })];
            videogameInstance.Genres.Clear();
            videogameInstance.Genres = [.. videogameDTO.Genres.Select(obj =>
            {
                var genre = _context.GenresSet.FirstOrDefault(it => it.Name == obj);
                if (genre == null)
                    genre = new Genre { Name = obj };
                return genre;
            })];

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

        [HttpPatch("json-patch-doc/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> JsonPatchDocVideogameInstance([FromRoute] int id, JsonPatchDocument<VideogameInstance> patchDocument)
        {
            var videogameInstance = await _context.VideogamesSet
                .Include(it => it.Platforms)
                .Include(it => it.Genres)
                .FirstOrDefaultAsync(it => it.Id == id);

            if (videogameInstance == null)
                return NotFound();

            if (patchDocument == null)
                return BadRequest();

            patchDocument.ApplyTo(videogameInstance, ModelState);

            videogameInstance.Version = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PatchVideogameInstance([FromRoute] int id, VideogamePatch videogame)
        {
            var videogameInstance = await _context.VideogamesSet
                .Include(it => it.Platforms)
                .Include(it => it.Genres)
                .FirstOrDefaultAsync(it => it.Id == id);

            if (videogameInstance == null)
                return NotFound();

            if (videogame.Name != null)
                videogameInstance.Name = videogame.Name;
            if (videogame.Description != null)
                videogameInstance.Description = videogame.Description;
            if (videogame.Platforms.Count > 0)
            {
                videogameInstance.Platforms.Clear();
                videogameInstance.Platforms = [.. videogame.Platforms.Select(platform =>
                {
                    var temp = _context.PlatformSet.FirstOrDefault(it => it.Name == platform);
                    temp ??= new Platform { Name = platform };
                    return temp;
                })];
            }
            if (videogame.Genres.Count > 0)
            {
                videogameInstance.Genres.Clear();
                videogameInstance.Genres = [.. videogame.Genres.Select(genre =>
                {
                    var temp = _context.GenresSet.FirstOrDefault(it => it.Name == genre);
                    temp ??= new Genre { Name = genre };
                    return temp;
                })];
            }

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
                Platforms = [.. videogameInstance.Platforms.Select(it => it.Name)],
                Genres = [.. videogameInstance.Genres.Select(it => it.Name)]
            };
        }
        private async Task<VideogameInstance> ToVideogameInstance(VideogamePostPutDTO videogamePostPutDTO)
        {
            Platform tempPlatform;
            Genre tempGenre;
            List<Platform> platforms = [];
            List<Genre> genres = [];

            foreach (string platform in videogamePostPutDTO.Platforms)
            {
                tempPlatform = await _context.PlatformSet.FirstOrDefaultAsync(it => it.Name == platform);

                tempPlatform ??= new Platform { Name = platform };

                platforms.Add(tempPlatform);
            }
            foreach (string genre in videogamePostPutDTO.Genres)
            {
                tempGenre = await _context.GenresSet.FirstOrDefaultAsync(it => it.Name == genre);

                tempGenre ??= new Genre{Name = genre};

                genres.Add(tempGenre);
            }

            return new VideogameInstance
            {
                Name = videogamePostPutDTO.Name,
                Description = videogamePostPutDTO.Description,
                Platforms = platforms,
                Genres = genres
            };
        }
    }
}
