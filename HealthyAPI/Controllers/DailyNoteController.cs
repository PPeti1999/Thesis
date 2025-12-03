using HealthyAPI.DTOs.CalendarSummary;
using HealthyAPI.DTOs.DailyNote;
using HealthyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HealthyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyNoteController : ControllerBase
    {
        private readonly IDailyNoteService _service;

        public DailyNoteController(IDailyNoteService service)
        {
            _service = service;
        }


        [HttpGet("summary/{userId}/{year}/{month}")]
        [Authorize]
        public async Task<ActionResult<List<CalendarSummaryDto>>> GetMonthlySummary(string userId, int year, int month)
        {
            var result = await _service.GetMonthlySummaryAsync(userId, year, month);
            return Ok(result);
        }

        [HttpGet("today")]
        [Authorize]
        public async Task<ActionResult<DailyNoteResponseDto>> GetToday()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

           
            var result = await _service.GetTodayNote(userId);

            if (result == null)
            {
               
                result = await _service.CreateDailyNote(userId);
            }

       

            return Ok(result);
        }


        [HttpGet("next/{userId}/{currentDate}")]
        public async Task<ActionResult<DailyNoteResponseDto>> GetNext(string userId, DateTime currentDate)
        {
            var dateOnly = DateTime.SpecifyKind(currentDate, DateTimeKind.Utc).ToLocalTime().Date;
            var note = await _service.GetNextNote(userId, dateOnly);
            if (note == null) return NotFound("There is no DailyNote created for the next day.");
            return Ok(note);
        }

        [HttpGet("previous/{userId}/{currentDate}")]
        public async Task<ActionResult<DailyNoteResponseDto>> GetPrevious(string userId, DateTime currentDate)
        {
            var dateOnly = DateTime.SpecifyKind(currentDate, DateTimeKind.Utc).ToLocalTime().Date;
            var note = await _service.GetPreviousNote(userId, dateOnly);
            if (note == null) return NotFound("There is no DailyNote for the previous day.");
            return Ok(note);
        }





        [HttpGet("by-date/{userId}/{date}")]
        [Authorize]
        public async Task<ActionResult<DailyNoteResponseDto>> GetByDate(string userId, DateTime date)
        {
            var dateOnly = DateTime.SpecifyKind(date, DateTimeKind.Utc).ToLocalTime().Date;
            var note = await _service.GetNoteByDate(userId, dateOnly);
            if (note == null) return NotFound("There is no DailyNote available on this date.");
            return Ok(note);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<DailyNoteResponseDto>> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.CreateDailyNote(userId);
            return CreatedAtAction(nameof(GetToday), new { }, result);
        }

        [HttpPut("{id}/weight")]
        [Authorize]
        public async Task<ActionResult<DailyNoteResponseDto>> UpdateWeight(string id, [FromBody] WeightUpdateDto dto)
        {
            var updated = await _service.UpdateWeight(id, dto.Weight);
            if (updated == null) return NotFound();

            var refreshed = await _service.GetTodayNote(updated.UserID);
            return Ok(refreshed);
        }
       
        [HttpGet("weight-history")]
        [Authorize]
        [ProducesResponseType(typeof(List<DailyNoteResponseDto>), 200)]
        public async Task<ActionResult<List<DailyNoteResponseDto>>> GetWeightHistory()
        {
           
            var weightHistory = await _service.GetAllDailyNotesForGraph();

         
            return Ok(weightHistory);
        }

    }
}
