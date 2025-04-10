using HealthyAPI.Data;
using HealthyAPI.DTOs.UserActivity;
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
    public class UserActivityController : ControllerBase
    {
        private readonly Context _context;

        public UserActivityController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserActivityResponseDto>>> GetAll()
        {
            var items = await _context.UserActivity
                .Include(ua => ua.ActivityCatalog)
                .Include(ua => ua.Photo)
                .ToListAsync();

            return Ok(items.Select(ua => new UserActivityResponseDto
            {
                UserActivityID = ua.UserActivityID,
                DailyNoteID = ua.DailyNoteID,
                ActivityCatalogID = ua.ActivityCatalogID,
                ActivityName = ua.ActivityCatalog?.Name,
                Duration = ua.Duration,
                Calories = ua.Calories,
                PhotoID = ua.PhotoID,
                PhotoData = ua.Photo?.PhotoData
            }));
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserActivityResponseDto>> Create(UserActivityCreateDto dto)
        {
            var activity = await _context.ActivityCatalog.FindAsync(dto.ActivityCatalogID);
            if (activity == null) return BadRequest("Invalid activity catalog ID");

            var calculatedCalories = (int)(dto.Duration / (float)activity.Minute * activity.Calories);

            var entity = new UserActivity
            {
                UserActivityID = Guid.NewGuid().ToString(),
                DailyNoteID = dto.DailyNoteID,
                ActivityCatalogID = dto.ActivityCatalogID,
                Duration = dto.Duration,
                Calories = calculatedCalories,
                PhotoID = dto.PhotoID
            };

            _context.UserActivity.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.UserActivityID }, new UserActivityResponseDto
            {
                UserActivityID = entity.UserActivityID,
                DailyNoteID = entity.DailyNoteID,
                ActivityCatalogID = entity.ActivityCatalogID,
                ActivityName = activity.Name,
                Duration = entity.Duration,
                Calories = entity.Calories,
                PhotoID = entity.PhotoID,
                PhotoData = null
            });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserActivityResponseDto>> GetById(string id)
        {
            var entity = await _context.UserActivity
                .Include(ua => ua.ActivityCatalog)
                .Include(ua => ua.Photo)
                .FirstOrDefaultAsync(ua => ua.UserActivityID == id);

            if (entity == null) return NotFound();

            return Ok(new UserActivityResponseDto
            {
                UserActivityID = entity.UserActivityID,
                DailyNoteID = entity.DailyNoteID,
                ActivityCatalogID = entity.ActivityCatalogID,
                ActivityName = entity.ActivityCatalog?.Name,
                Duration = entity.Duration,
                Calories = entity.Calories,
                PhotoID = entity.PhotoID,
                PhotoData = entity.Photo?.PhotoData
            });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<UserActivityResponseDto>> Update(string id, UserActivityCreateDto dto)
        {
            var entity = await _context.UserActivity.Include(ua => ua.ActivityCatalog).FirstOrDefaultAsync(ua => ua.UserActivityID == id);
            if (entity == null) return NotFound();

            var activity = await _context.ActivityCatalog.FindAsync(dto.ActivityCatalogID);
            if (activity == null) return BadRequest("Invalid activity catalog ID");

            entity.DailyNoteID = dto.DailyNoteID;
            entity.ActivityCatalogID = dto.ActivityCatalogID;
            entity.Duration = dto.Duration;
            entity.Calories = (int)(dto.Duration / (float)activity.Minute * activity.Calories);
            entity.PhotoID = dto.PhotoID;

            await _context.SaveChangesAsync();

            return Ok(new UserActivityResponseDto
            {
                UserActivityID = entity.UserActivityID,
                DailyNoteID = entity.DailyNoteID,
                ActivityCatalogID = entity.ActivityCatalogID,
                ActivityName = activity.Name,
                Duration = entity.Duration,
                Calories = entity.Calories,
                PhotoID = entity.PhotoID,
                PhotoData = entity.Photo?.PhotoData
            });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _context.UserActivity.FindAsync(id);
            if (entity == null) return NotFound();

            _context.UserActivity.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

