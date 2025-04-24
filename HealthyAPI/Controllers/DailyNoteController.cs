using HealthyAPI.DTOs.DailyNote;
using HealthyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            else
            {
                // 🔥 Frissítjük a napi tápanyagokat, ha már van napló
                var dailyNoteId = result.DailyNoteID;
                await _service.UpdateMealNutritionAsync(dailyNoteId);
                // újra lekérjük frissítve
                result = await _service.GetTodayNote(userId);
            }
            return Ok(result);
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

        
    }
}
