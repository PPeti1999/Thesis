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
        private readonly IMealEntriesService _service;

        public MealEntriesController(IMealEntriesService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MealEntryResponseDto>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<MealEntryResponseDto>> GetById(string id)
        {
            var result = await _service.GetById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpGet("daily-note/{dailyNoteId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MealEntryResponseDto>>> GetByDailyNote(string dailyNoteId)
        {
            var results = await _service.GetByDailyNoteId(dailyNoteId);
            return Ok(results);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MealEntryResponseDto>> Create(MealEntryCreateDto dto)
        {
            var created = await _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.MealEntryID }, created);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<MealEntryResponseDto>> Update(string id, MealEntryCreateDto dto)
        {
            var updated = await _service.Update(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _service.Delete(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}

