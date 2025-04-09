using HealthyAPI.Data;
using HealthyAPI.DTOs.Food;
using HealthyAPI.DTOs.MealEntries;
using HealthyAPI.Models;
using HealthyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealEntriesController : ControllerBase
    {
        private readonly Context _context;

        public MealEntriesController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MealEntryResponseDto>>> GetAll()
        {
            var entries = await _context.MealEntries
                .Include(me => me.MealType)
                .Include(me => me.DailyNote)
                .ToListAsync();

            var result = entries.Select(e => new MealEntryResponseDto
            {
                MealEntryID = e.MealEntryID,
                DailyNoteID = e.DailyNoteID,
                DailyNoteCreatedAt = e.DailyNote?.CreatedAt,
                MealTypeID = e.MealTypeID,
                MealTypeName = e.MealType?.Name
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<MealEntryResponseDto>> GetById(string id)
        {
            var e = await _context.MealEntries
                .Include(me => me.MealType)
                .Include(me => me.DailyNote)
                .FirstOrDefaultAsync(me => me.MealEntryID == id);

            if (e == null) return NotFound();

            return Ok(new MealEntryResponseDto
            {
                MealEntryID = e.MealEntryID,
                DailyNoteID = e.DailyNoteID,
                DailyNoteCreatedAt = e.DailyNote?.CreatedAt,
                MealTypeID = e.MealTypeID,
                MealTypeName = e.MealType?.Name
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MealEntryResponseDto>> Create([FromBody] MealEntryCreateDto dto)
        {
            var newEntry = new MealEntries
            {
                MealEntryID = Guid.NewGuid().ToString(),
                DailyNoteID = dto.DailyNoteID,
                MealTypeID = dto.MealTypeID
            };

            _context.MealEntries.Add(newEntry);
            await _context.SaveChangesAsync();

            var created = await _context.MealEntries
                .Include(me => me.MealType)
                .Include(me => me.DailyNote)
                .FirstOrDefaultAsync(me => me.MealEntryID == newEntry.MealEntryID);

            return CreatedAtAction(nameof(GetById), new { id = created.MealEntryID }, new MealEntryResponseDto
            {
                MealEntryID = created.MealEntryID,
                DailyNoteID = created.DailyNoteID,
                DailyNoteCreatedAt = created.DailyNote?.CreatedAt,
                MealTypeID = created.MealTypeID,
                MealTypeName = created.MealType?.Name
            });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<MealEntryResponseDto>> Update(string id, [FromBody] MealEntryCreateDto dto)
        {
            var entry = await _context.MealEntries.FindAsync(id);
            if (entry == null) return NotFound();

            entry.DailyNoteID = dto.DailyNoteID;
            entry.MealTypeID = dto.MealTypeID;

            await _context.SaveChangesAsync();

            var updated = await _context.MealEntries
                .Include(me => me.MealType)
                .Include(me => me.DailyNote)
                .FirstOrDefaultAsync(me => me.MealEntryID == id);

            return Ok(new MealEntryResponseDto
            {
                MealEntryID = updated.MealEntryID,
                DailyNoteID = updated.DailyNoteID,
                DailyNoteCreatedAt = updated.DailyNote?.CreatedAt,
                MealTypeID = updated.MealTypeID,
                MealTypeName = updated.MealType?.Name
            });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var entry = await _context.MealEntries.FindAsync(id);
            if (entry == null) return NotFound();

            _context.MealEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

