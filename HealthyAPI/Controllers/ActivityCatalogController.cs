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
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityCatalogResponseDto>> GetById(string id)
        {
            var activity = await _service.GetById(id);
            if (activity == null) return NotFound();
            return Ok(activity);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ActivityCatalogResponseDto>> Create(ActivityCatalogCreateDto dto)
        {
            var created = await _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ActivityCatalogID }, created);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ActivityCatalogResponseDto>> Update(string id, ActivityCatalogCreateDto dto)
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
