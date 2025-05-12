using HealthyAPI.Data;
using HealthyAPI.DTOs.Recipe;
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
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _service;

        public RecipesController(IRecipeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeResponseDto>>> GetAll()
        {
            var list = await _service.GetAll();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeResponseDto>> GetById(string id)
        {
            var recipe = await _service.GetById(id);
            if (recipe == null) return NotFound();
            return Ok(recipe);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RecipeResponseDto>> Create([FromBody] RecipeCreateDto dto)
        {
            var created = await _service.Create(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<RecipeResponseDto>> Update(string id, [FromBody] RecipeUpdateDto dto)
        {
            var updated = await _service.Update(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Szerverhiba történt: " + ex.Message });
            }
        }
    }
}
