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
            return await _context.VideogamesSet.Select(it => ToPublishableDTO(it)).ToListAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<VideogamePublishableDTO>> GetVideogameInstance(int id)
        {
            var videogameInstance = await _context.VideogamesSet.FindAsync(id);

            if (videogameInstance == null)
                return NotFound();

            return ToPublishableDTO(videogameInstance);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PostVideogameInstance(VideogamePostPutDTO videogameDTO)
        {
            var videogameInstance = ToVideogameInstance(0, videogameDTO);
            _context.VideogamesSet.Add(videogameInstance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVideogameInstance), new { id = videogameInstance.Id }, videogameInstance);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PutVideogameInstance(int id, VideogamePostPutDTO videogameDTO)
        {
            var videogameInstance = await _context.VideogamesSet.FindAsync(id);
            
            if (videogameInstance == null)
                return BadRequest();
            
            videogameInstance.Name = videogameDTO.Name;
            videogameInstance.Description = videogameDTO.Description;
            videogameInstance.Platform = videogameDTO.Platform;

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
        public async Task<IActionResult> PatchVideogameInstance(int id, JsonPatchDocument<VideogameInstance> patchDocument)
        {
            var videogameInstance = await _context.VideogamesSet.FindAsync(id);

            if (videogameInstance == null)
                return NotFound();

            if (patchDocument == null)
                return BadRequest();

            patchDocument.ApplyTo(videogameInstance, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
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
        private VideogamePublishableDTO ToPublishableDTO(VideogameInstance videogameInstance)
        {
            return new VideogamePublishableDTO
            {
                Id = videogameInstance.Id,
                Name = videogameInstance.Name,
                Description = videogameInstance.Description,
                Platform = videogameInstance.Platform
            };
        }
        private VideogameInstance ToVideogameInstance(int id, VideogamePostPutDTO videogamePostPutDTO)
        {
            return new VideogameInstance
            {
                Id = id,
                Name = videogamePostPutDTO.Name,
                Description = videogamePostPutDTO.Description,
                Platform = videogamePostPutDTO.Platform
            };
        }
    }
}
