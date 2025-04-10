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

namespace HealthyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly Context _context;

        public RecipesController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<RecipeResponseDto>>> GetAll()
        {
            var recipes = await _context.Recipe.Include(r => r.Photo).ToListAsync();

            var results = new List<RecipeResponseDto>();

            foreach (var r in recipes)
            {
                await RecalculateNutrition(r);
                results.Add(await MapToDto(r));
            }

            return Ok(results);
        }

        [HttpGet("{id}")]
        //[Authorize]
        public async Task<ActionResult<RecipeResponseDto>> GetById(string id)
        {
            var r = await _context.Recipe.Include(r => r.Photo).FirstOrDefaultAsync(r => r.RecipeID == id);
            if (r == null) return NotFound();
            await RecalculateNutrition(r);
            return Ok(await MapToDto(r));
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RecipeResponseDto>> Create(RecipeCreateDto dto)
        {
            var recipe = new Recipe
            {
                RecipeID = Guid.NewGuid().ToString(),
                Title = dto.Title,
                Description = dto.Description,
                PhotoID = dto.PhotoID,
                CreatedAt = new DateTime(2024, 4, 9)
            };

            _context.Recipe.Add(recipe);
            await _context.SaveChangesAsync();

            foreach (var item in dto.Ingredients)
            {
                _context.RecipeFoods.Add(new RecipeFoods
                {
                    RecipeFoodID = Guid.NewGuid().ToString(),
                    RecipeID = recipe.RecipeID,
                    FoodID = item.FoodID,
                    Quantity = item.Quantity
                });
            }

            await _context.SaveChangesAsync();
            await RecalculateNutrition(recipe);

            return CreatedAtAction(nameof(GetById), new { id = recipe.RecipeID }, await MapToDto(recipe));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<RecipeResponseDto>> Update(string id, RecipeUpdateDto dto)
        {
            var recipe = await _context.Recipe.FindAsync(id);
            if (recipe == null) return NotFound();

            recipe.Title = dto.Title;
            recipe.Description = dto.Description;
            recipe.PhotoID = dto.PhotoID;

            var existingIngredients = _context.RecipeFoods.Where(rf => rf.RecipeID == id);
            _context.RecipeFoods.RemoveRange(existingIngredients);

            foreach (var item in dto.Ingredients)
            {
                _context.RecipeFoods.Add(new RecipeFoods
                {
                    RecipeFoodID = Guid.NewGuid().ToString(),
                    RecipeID = recipe.RecipeID,
                    FoodID = item.FoodID,
                    Quantity = item.Quantity
                });
            }

            await _context.SaveChangesAsync();
            await RecalculateNutrition(recipe);

            _context.Recipe.Update(recipe);
            await _context.SaveChangesAsync();

            return Ok(await MapToDto(recipe));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var recipe = await _context.Recipe.FindAsync(id);
            if (recipe == null) return NotFound();

            var ingredients = _context.RecipeFoods.Where(rf => rf.RecipeID == id);
            _context.RecipeFoods.RemoveRange(ingredients);
            _context.Recipe.Remove(recipe);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task RecalculateNutrition(Recipe recipe)
        {
            var recipeFoods = await _context.RecipeFoods.Include(rf => rf.Food)
                .Where(rf => rf.RecipeID == recipe.RecipeID).ToListAsync();

            recipe.SumProtein = recipeFoods.Sum(rf => rf.Quantity / 100 * rf.Food.Protein);
            recipe.SumCarb = recipeFoods.Sum(rf => rf.Quantity / 100 * rf.Food.Carb);
            recipe.SumFat = recipeFoods.Sum(rf => rf.Quantity / 100 * rf.Food.Fat);
            recipe.SumCalorie = recipeFoods.Sum(rf => rf.Quantity / 100 * rf.Food.Calorie);

            _context.Recipe.Update(recipe);
            await _context.SaveChangesAsync();
        }

        private async Task<RecipeResponseDto> MapToDto(Recipe recipe)
        {
            var photo = await _context.Photo.FindAsync(recipe.PhotoID);
            var ingredients = await _context.RecipeFoods
                .Include(rf => rf.Food)
                .Where(rf => rf.RecipeID == recipe.RecipeID)
                .Select(rf => new RecipeIngredientDetailDto
                {
                    FoodID = rf.FoodID,
                    FoodName = rf.Food.Title,
                    Quantity = rf.Quantity
                }).ToListAsync();

            return new RecipeResponseDto
            {
                RecipeID = recipe.RecipeID,
                Title = recipe.Title,
                Description = recipe.Description,
                SumProtein = recipe.SumProtein,
                SumCarb = recipe.SumCarb,
                SumFat = recipe.SumFat,
                SumCalorie = recipe.SumCalorie,
                PhotoID = recipe.PhotoID,
                PhotoData = photo?.PhotoData,
                CreatedAt = recipe.CreatedAt,
                Ingredients = ingredients
            };
        }
    }
}
