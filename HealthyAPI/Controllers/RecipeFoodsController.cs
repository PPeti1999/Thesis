using HealthyAPI.Data;
using HealthyAPI.DTOs.RecipeFood;
using HealthyAPI.Models;
using HealthyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeFoodsController : ControllerBase
    {
        private readonly IRecipeFoodService _service;

        public RecipeFoodsController(IRecipeFoodService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RecipeFoodResponseDto>>> GetAll()
        {
            var items = await _service.GetAll();
            return Ok(items.Select(rf => new RecipeFoodResponseDto
            {
                RecipeFoodID = rf.RecipeFoodID,
                RecipeID = rf.RecipeID,
                FoodID = rf.FoodID,
                FoodName = rf.Food?.Title,
                Quantity = rf.Quantity
            }));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<RecipeFoodResponseDto>> GetById(string id)
        {
            var rf = await _service.GetById(id);
            if (rf == null) return NotFound();

            return Ok(new RecipeFoodResponseDto
            {
                RecipeFoodID = rf.RecipeFoodID,
                RecipeID = rf.RecipeID,
                FoodID = rf.FoodID,
                FoodName = rf.Food?.Title,
                Quantity = rf.Quantity
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RecipeFoodResponseDto>> Create(RecipeFoodCreateDto dto)
        {
            var entity = new RecipeFoods
            {
                RecipeID = dto.RecipeID,
                FoodID = dto.FoodID,
                Quantity = dto.Quantity
            };

            var created = await _service.Create(entity);
            return CreatedAtAction(nameof(GetById), new { id = created.RecipeFoodID }, new RecipeFoodResponseDto
            {
                RecipeFoodID = created.RecipeFoodID,
                RecipeID = created.RecipeID,
                FoodID = created.FoodID,
                FoodName = created.Food?.Title,
                Quantity = created.Quantity
            });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<RecipeFoodResponseDto>> Update(string id, RecipeFoodCreateDto dto)
        {
            var updated = new RecipeFoods
            {
                RecipeID = dto.RecipeID,
                FoodID = dto.FoodID,
                Quantity = dto.Quantity
            };

            var result = await _service.Update(id, updated);
            if (result == null) return NotFound();

            return Ok(new RecipeFoodResponseDto
            {
                RecipeFoodID = result.RecipeFoodID,
                RecipeID = result.RecipeID,
                FoodID = result.FoodID,
                FoodName = result.Food?.Title,
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
