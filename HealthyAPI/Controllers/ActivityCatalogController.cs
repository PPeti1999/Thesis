using HealthyAPI.Data;
using HealthyAPI.DTOs.ActivityCatalog;
using HealthyAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HealthyAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ActivityCatalogController : ControllerBase
    {
        private readonly Context _context;

        public ActivityCatalogController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityCatalogResponseDto>>> GetAll()
        {
            var activities = await _context.ActivityCatalog.ToListAsync();

            return Ok(activities.Select(a => new ActivityCatalogResponseDto
            {
                ActivityCatalogID = a.ActivityCatalogID,
                Name = a.Name,
                Minute = a.Minute,
                Calories = a.Calories,
                CreatedAt = a.CreatedAt
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityCatalogResponseDto>> GetById(string id)
        {
            var activity = await _context.ActivityCatalog.FindAsync(id);
            if (activity == null) return NotFound();

            return Ok(new ActivityCatalogResponseDto
            {
                ActivityCatalogID = activity.ActivityCatalogID,
                Name = activity.Name,
                Minute = activity.Minute,
                Calories = activity.Calories,
                CreatedAt = activity.CreatedAt
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ActivityCatalogResponseDto>> Create(ActivityCatalogCreateDto dto)
        {
            var activity = new ActivityCatalog
            {
                ActivityCatalogID = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Minute = dto.Minute,
                Calories = dto.Calories,
                CreatedAt = DateTime.UtcNow
            };

            _context.ActivityCatalog.Add(activity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = activity.ActivityCatalogID }, new ActivityCatalogResponseDto
            {
                ActivityCatalogID = activity.ActivityCatalogID,
                Name = activity.Name,
                Minute = activity.Minute,
                Calories = activity.Calories,
                CreatedAt = activity.CreatedAt
            });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ActivityCatalogResponseDto>> Update(string id, ActivityCatalogCreateDto dto)
        {
            var entity = await _context.ActivityCatalog.FindAsync(id);
            if (entity == null) return NotFound();

            entity.Name = dto.Name;
            entity.Minute = dto.Minute;
            entity.Calories = dto.Calories;

            await _context.SaveChangesAsync();

            return Ok(new ActivityCatalogResponseDto
            {
                ActivityCatalogID = entity.ActivityCatalogID,
                Name = entity.Name,
                Minute = entity.Minute,
                Calories = entity.Calories,
                CreatedAt = entity.CreatedAt
            });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var activity = await _context.ActivityCatalog.FindAsync(id);
            if (activity == null) return NotFound();

            _context.ActivityCatalog.Remove(activity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
