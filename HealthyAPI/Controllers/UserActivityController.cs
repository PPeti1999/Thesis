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
using HealthyAPI.Services;

namespace HealthyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserActivityController : ControllerBase
    {
        private readonly IUserActivityService _service;

        public UserActivityController(IUserActivityService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserActivityResponseDto>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserActivityResponseDto>> GetById(string id)
        {
            var activity = await _service.GetById(id);
            if (activity == null) return NotFound();
            return Ok(activity);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserActivityResponseDto>> Create(UserActivityCreateDto dto)
        {
            var created = await _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.UserActivityID }, created);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<UserActivityResponseDto>> Update(string id, UserActivityCreateDto dto)
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

