using HealthyAPI.Data;
using HealthyAPI.DTOs.Recipe;
using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace HealthyAPI.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly Context _context;

        public RecipeService(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RecipeResponseDto>> GetAll()
        {
            var recipes = await _context.Recipe.Include(r => r.Photo).ToListAsync();
            var result = new List<RecipeResponseDto>();

            foreach (var r in recipes)
            {
                await RecalculateNutrition(r);
                result.Add(await MapToDto(r));
            }
            return result;
        }

        public async Task<RecipeResponseDto?> GetById(string id)
        {
            var recipe = await _context.Recipe.Include(r => r.Photo).FirstOrDefaultAsync(r => r.RecipeID == id);
            if (recipe == null) return null;
            await RecalculateNutrition(recipe);
            return await MapToDto(recipe);
        }

        public async Task<RecipeResponseDto> Create(RecipeCreateDto dto)
        {
            var recipe = new Recipe
            {
                RecipeID = Guid.NewGuid().ToString(),
                Title = dto.Title,
                Description = dto.Description,
                PhotoID = dto.PhotoID,
                CreatedAt = DateTime.UtcNow
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
            return await MapToDto(recipe);
        }

        public async Task<RecipeResponseDto?> Update(string id, RecipeUpdateDto dto)
        {
            var recipe = await _context.Recipe.FindAsync(id);
            if (recipe == null) return null;

            recipe.Title = dto.Title;
            recipe.Description = dto.Description;
            recipe.PhotoID = dto.PhotoID;

            _context.RecipeFoods.RemoveRange(_context.RecipeFoods.Where(rf => rf.RecipeID == id));
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

            return await MapToDto(recipe);
        }

        public async Task<bool> Delete(string id)
        {
            var recipe = await _context.Recipe.FindAsync(id);
            if (recipe == null) return false;

            _context.RecipeFoods.RemoveRange(_context.RecipeFoods.Where(rf => rf.RecipeID == id));
            _context.Recipe.Remove(recipe);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task RecalculateNutrition(Recipe recipe)
        {
            var recipeFoods = await _context.RecipeFoods.Include(rf => rf.Food)
                .Where(rf => rf.RecipeID == recipe.RecipeID).ToListAsync();

            recipe.SumProtein = recipeFoods.Sum(rf => rf.Quantity / 100 * rf.Food.Protein);
            recipe.SumCarb = recipeFoods.Sum(rf => rf.Quantity / 100 * rf.Food.Carb);
            recipe.SumFat = recipeFoods.Sum(rf => rf.Quantity / 100 * rf.Food.Fat);
            recipe.SumCalorie = recipeFoods.Sum(rf => rf.Quantity / 100 * rf.Food.Calorie);
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
