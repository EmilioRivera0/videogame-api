using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using videogame_api.src.Models;
using videogame_api.src.DTO;
using Microsoft.AspNetCore.JsonPatch;

namespace videogame_api.src.Controllers
{
    [Route("api/[controller]")]
    public class PlatformsController(AppDbContext context) : ControllerBase
    {
        // member fields
        private readonly AppDbContext _context = context;

        // action methods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlatformPublishableDTO>>> GetPlatformsSet()
        {
            return await _context.PlatformSet.Select(it => ToPublishableDTO(it)).ToListAsync();
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<PlatformPublishableDTO>> GetPlatform(int id)
        {
            var platform = await _context.PlatformSet.FindAsync(id);

            if (platform == null)
                return NotFound();

            return ToPublishableDTO(platform);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PostPlatform(PlatformPostPutDTO platform)
        {
            var platformInstance = ToPlatformInstance(platform);
            _context.PlatformSet.Add(platformInstance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlatform), new { id = platformInstance.Id }, ToPublishableDTO(platformInstance));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PutPlatform(int id, PlatformPostPutDTO platform)
        {
            var platformInstance = await _context.PlatformSet.FindAsync(id);

            if (platformInstance == null)
                return NotFound();

            platformInstance.Name = platform.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlatformExists(id))
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
        public async Task<IActionResult> PatchPlatform([FromRoute] int id, JsonPatchDocument<Platform> patchDocument)
        {
            var platform = await _context.PlatformSet.FindAsync(id);
            
            if (platform == null)
                return NotFound();

            if (patchDocument == null)
                return BadRequest();
            
            patchDocument.ApplyTo(platform, ModelState);
            
            platform.Version = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeletePlatform(int id)
        {
            var platform = await _context.PlatformSet.FindAsync(id);
            if (platform == null)
                return NotFound();

            _context.PlatformSet.Remove(platform);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // member methods
        private bool PlatformExists(int id) => _context.PlatformSet.Any(e => e.Id == id);
        private static PlatformPublishableDTO ToPublishableDTO(Platform platform)
        {
            return new PlatformPublishableDTO
            {
                Id = platform.Id,
                Name = platform.Name,
            };
        }
        private static Platform ToPlatformInstance(PlatformPostPutDTO platformDTO)
        {
            return new Platform
            {
                Name = platformDTO.Name,
            };
        }
    }
}
