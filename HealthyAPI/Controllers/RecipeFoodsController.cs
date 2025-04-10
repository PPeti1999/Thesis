using HealthyAPI.Data;
using HealthyAPI.DTOs.RecipeFood;
using HealthyAPI.Models;
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
        private readonly Context _context;

        public RecipeFoodsController(Context context)
        {
            _context = context;
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RecipeFoodResponseDto>>> GetAll()
        {
            var items = await _context.RecipeFoods.Include(rf => rf.Food).ToListAsync();
            var result = items.Select(entity => new RecipeFoodResponseDto
            {
                RecipeFoodID = entity.RecipeFoodID,
                RecipeID = entity.RecipeID,
                FoodID = entity.FoodID,
                FoodName = entity.Food?.Title,
                Quantity = entity.Quantity
            });
            return Ok(result);
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

            _context.RecipeFoods.Add(entity);
            await _context.SaveChangesAsync();

            var food = await _context.Food.FindAsync(dto.FoodID);

            return CreatedAtAction(nameof(GetById), new { id = entity.RecipeFoodID }, new RecipeFoodResponseDto
            {
                RecipeFoodID = entity.RecipeFoodID,
                RecipeID = entity.RecipeID,
                FoodID = entity.FoodID,
                FoodName = food?.Title,
                Quantity = entity.Quantity
            });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<RecipeFoodResponseDto>> GetById(string id)
        {
            var entity = await _context.RecipeFoods.Include(rf => rf.Food).FirstOrDefaultAsync(rf => rf.RecipeFoodID == id);
            if (entity == null) return NotFound();

            return Ok(new RecipeFoodResponseDto
            {
                RecipeFoodID = entity.RecipeFoodID,
                RecipeID = entity.RecipeID,
                FoodID = entity.FoodID,
                FoodName = entity.Food?.Title,
                Quantity = entity.Quantity
            });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<RecipeFoodResponseDto>> Update(string id, RecipeFoodCreateDto dto)
        {
            var entity = await _context.RecipeFoods.Include(rf => rf.Food).FirstOrDefaultAsync(rf => rf.RecipeFoodID == id);
            if (entity == null) return NotFound();

            entity.RecipeID = dto.RecipeID;
            entity.FoodID = dto.FoodID;
            entity.Quantity = dto.Quantity;

            await _context.SaveChangesAsync();

            return Ok(new RecipeFoodResponseDto
            {
                RecipeFoodID = entity.RecipeFoodID,
                RecipeID = entity.RecipeID,
                FoodID = entity.FoodID,
                FoodName = entity.Food?.Title,
                Quantity = entity.Quantity
            });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _context.RecipeFoods.FindAsync(id);
            if (entity == null) return NotFound();

            _context.RecipeFoods.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
