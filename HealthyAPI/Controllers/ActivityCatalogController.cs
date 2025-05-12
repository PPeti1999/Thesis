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
using HealthyAPI.Services;
using HealthyAPI.DTOs.Food;

namespace HealthyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityCatalogController : ControllerBase
    {
        private readonly IActivityCatalogService _service;

        public ActivityCatalogController(IActivityCatalogService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityCatalogResponseDto>>> GetAll()
        {
            var activityCatalogs = await _service.GetAll();
            return Ok(activityCatalogs.Select(f => new ActivityCatalogResponseDto
            {
                ActivityCatalogID = f.ActivityCatalogID,
                Name = f.Name,
                Minute = f.Minute,
                Calories = f.Calories,
                CreatedAt = f.CreatedAt
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityCatalogResponseDto>> GetById(string id)
        {
            var activityCatalogs = await _service.GetById(id);
            if (activityCatalogs == null) return NotFound();
            return Ok(new ActivityCatalogResponseDto
            {
                ActivityCatalogID = activityCatalogs.ActivityCatalogID,
                Name = activityCatalogs.Name,
                Minute = activityCatalogs.Minute,
                Calories = activityCatalogs.Calories,
                CreatedAt = activityCatalogs.CreatedAt
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ActivityCatalogResponseDto>> Create([FromBody] ActivityCatalogCreateDto dto)
        {


            var activityCatalog = new ActivityCatalog
            {
                ActivityCatalogID = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Minute = dto.Minute,
                Calories = dto.Calories,
                CreatedAt = DateTime.UtcNow
            };


            var activityCatalogs = await _service.Create(activityCatalog);
            
            return Ok(new ActivityCatalogResponseDto
            {
                ActivityCatalogID = activityCatalogs.ActivityCatalogID,
                Name = activityCatalogs.Name,
                Minute = activityCatalogs.Minute,
                Calories = activityCatalogs.Calories,
                CreatedAt = activityCatalogs.CreatedAt
            });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ActivityCatalogResponseDto>> Update(string id, [FromBody] ActivityCatalogCreateDto dto)
        {
            var activityCatalogs = await _service.GetById(id);
            if (activityCatalogs == null) return NotFound();
            activityCatalogs.Name = dto.Name;
            activityCatalogs.Minute = dto.Minute;
            activityCatalogs.Calories = dto.Calories;
                
         


            var activityCatalog = await _service.Update(id, activityCatalogs);

            return Ok(new ActivityCatalogResponseDto
            {
                ActivityCatalogID = activityCatalog.ActivityCatalogID,
                Name = activityCatalog.Name,
                Minute = activityCatalog.Minute,
                Calories = activityCatalog.Calories,
                CreatedAt = activityCatalog.CreatedAt
            });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var success = await _service.Delete(id);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
