using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using videogame_api.src.Models;
using videogame_api.src.DTO;
using Microsoft.AspNetCore.JsonPatch;

namespace videogame_api.src.Controllers
{
    [Route("api/[controller]")]
    public class GenresController(AppDbContext context) : ControllerBase
    {
        // member fields
        private readonly AppDbContext _context = context;

        // action methods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenrePublishableDTO>>> GetGenresSet()
        {
            return await _context.GenresSet.Select(it => ToPublishableDTO(it)).ToListAsync();
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<GenrePublishableDTO>> GetGenre(int id)
        {
            var genre = await _context.GenresSet.FindAsync(id);

            if (genre == null)
                return NotFound();

            return ToPublishableDTO(genre);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PostGenre(GenrePostPutDTO genre)
        {
            var genreInstance = ToGenreInstance(genre);
            _context.GenresSet.Add(genreInstance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGenre), new { id = genreInstance.Id }, ToPublishableDTO(genreInstance));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PutGenre(int id, GenrePostPutDTO genre)
        {
            var genreInstance = await _context.GenresSet.FindAsync(id);

            if (genreInstance == null)
                return NotFound();

            genreInstance.Name = genre.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(id))
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
        public async Task<IActionResult> PatchGenre([FromRoute] int id, JsonPatchDocument<Genre> patchDocument)
        {
            var genre = await _context.GenresSet.FindAsync(id);
            
            if (genre == null)
                return NotFound();

            if (patchDocument == null)
                return BadRequest();
            
            patchDocument.ApplyTo(genre);
            
            genre.Version = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _context.GenresSet.FindAsync(id);
            if (genre == null)
                return NotFound();

            _context.GenresSet.Remove(genre);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // member methods
        private bool GenreExists(int id) => _context.GenresSet.Any(e => e.Id == id);
        private static GenrePublishableDTO ToPublishableDTO(Genre genre)
        {
            return new GenrePublishableDTO
            {
                Id = genre.Id,
                Name = genre.Name,
            };
        }
        private static Genre ToGenreInstance(GenrePostPutDTO genreDTO)
        {
            return new Genre
            {
                Name = genreDTO.Name,
            };
        }
    }
}
