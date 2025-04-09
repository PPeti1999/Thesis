using HealthyAPI.Data;
using HealthyAPI.DTOs.MealType;
using HealthyAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealTypesController : ControllerBase
    {
        private readonly Context _context;

        public MealTypesController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MealTypeResponseDto>>> GetAll()
        {
            var list = await _context.MealTypes.Include(mt => mt.Photo).ToListAsync();

            return Ok(list.Select(mt => new MealTypeResponseDto
            {
                MealTypeID = mt.MealTypeID,
                Name = mt.Name,
                PhotoID = mt.PhotoID,
                PhotoData = mt.Photo?.PhotoData
            }));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<MealTypeResponseDto>> GetById(string id)
        {
            var mt = await _context.MealTypes.Include(m => m.Photo).FirstOrDefaultAsync(m => m.MealTypeID == id);
            if (mt == null) return NotFound();

            return Ok(new MealTypeResponseDto
            {
                MealTypeID = mt.MealTypeID,
                Name = mt.Name,
                PhotoID = mt.PhotoID,
                PhotoData = mt.Photo?.PhotoData
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MealTypeResponseDto>> Create(MealTypeCreateDto dto)
        {
            var entity = new MealTypes
            {
                Name = dto.Name,
                PhotoID = dto.PhotoID
            };

            _context.MealTypes.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.MealTypes.Include(mt => mt.Photo).FirstOrDefaultAsync(m => m.MealTypeID == entity.MealTypeID);

            return CreatedAtAction(nameof(GetById), new { id = entity.MealTypeID }, new MealTypeResponseDto
            {
                MealTypeID = created.MealTypeID,
                Name = created.Name,
                PhotoID = created.PhotoID,
                PhotoData = created.Photo?.PhotoData
            });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<MealTypeResponseDto>> Update(string id, MealTypeCreateDto dto)
        {
            var entity = await _context.MealTypes.Include(m => m.Photo).FirstOrDefaultAsync(m => m.MealTypeID == id);
            if (entity == null) return NotFound();

            entity.Name = dto.Name;
            entity.PhotoID = dto.PhotoID;
            await _context.SaveChangesAsync();

            return Ok(new MealTypeResponseDto
            {
                MealTypeID = entity.MealTypeID,
                Name = entity.Name,
                PhotoID = entity.PhotoID,
                PhotoData = entity.Photo?.PhotoData
            });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _context.MealTypes.FindAsync(id);
            if (entity == null) return NotFound();

            _context.MealTypes.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

