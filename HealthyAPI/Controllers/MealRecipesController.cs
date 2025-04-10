using HealthyAPI.Data;
using HealthyAPI.DTOs.MealRecipe;
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
    public class MealRecipesController : ControllerBase
    {
        private readonly IMealRecipesService _service;

        public MealRecipesController(IMealRecipesService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MealRecipeResponseDto>>> GetAll()
        {
            var items = await _service.GetAll();
            return Ok(items.Select(mr => new MealRecipeResponseDto
            {
                MealRecipeID = mr.MealRecipeID,
                MealEntryID = mr.MealEntryID,
                RecipeID = mr.RecipeID,
                RecipeTitle = mr.Recipe?.Title,
                Quantity = mr.Quantity
            }));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<MealRecipeResponseDto>> GetById(string id)
        {
            var mr = await _service.GetById(id);
            if (mr == null) return NotFound();

            return Ok(new MealRecipeResponseDto
            {
                MealRecipeID = mr.MealRecipeID,
                MealEntryID = mr.MealEntryID,
                RecipeID = mr.RecipeID,
                RecipeTitle = mr.Recipe?.Title,
                Quantity = mr.Quantity
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MealRecipeResponseDto>> Create(MealRecipeCreateDto dto)
        {
            var entity = new MealRecipes
            {
                MealRecipeID = Guid.NewGuid().ToString(),
                MealEntryID = dto.MealEntryID,
                RecipeID = dto.RecipeID,
                Quantity = dto.Quantity
            };

            var created = await _service.Create(entity);
            return CreatedAtAction(nameof(GetById), new { id = created.MealRecipeID }, new MealRecipeResponseDto
            {
                MealRecipeID = created.MealRecipeID,
                MealEntryID = created.MealEntryID,
                RecipeID = created.RecipeID,
                RecipeTitle = created.Recipe?.Title,
                Quantity = created.Quantity
            });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<MealRecipeResponseDto>> Update(string id, MealRecipeCreateDto dto)
        {
            var updated = new MealRecipes
            {
                MealEntryID = dto.MealEntryID,
                RecipeID = dto.RecipeID,
                Quantity = dto.Quantity
            };

            var result = await _service.Update(id, updated);
            if (result == null) return NotFound();

            return Ok(new MealRecipeResponseDto
            {
                MealRecipeID = result.MealRecipeID,
                MealEntryID = result.MealEntryID,
                RecipeID = result.RecipeID,
                RecipeTitle = result.Recipe?.Title,
                Quantity = result.Quantity
            });
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