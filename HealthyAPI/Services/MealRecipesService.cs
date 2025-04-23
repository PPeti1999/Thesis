using HealthyAPI.Data;
using HealthyAPI.DTOs.MealRecipe;
using HealthyAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public class MealRecipesService : IMealRecipesService
    {
        private readonly Context _context;

        public MealRecipesService(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MealRecipeResponseDto>> GetAll()
        {
            return await _context.MealRecipes
                .Include(mr => mr.Recipe)
                .Select(mr => new MealRecipeResponseDto
                {
                    MealRecipeID = mr.MealRecipeID,
                    MealEntryID = mr.MealEntryID,
                    RecipeID = mr.RecipeID,
                    RecipeTitle = mr.Recipe.Title,
                    Quantity = mr.Quantity
                })
                .ToListAsync();
        }

        public async Task<MealRecipeResponseDto?> GetById(string id)
        {
            var mr = await _context.MealRecipes.Include(r => r.Recipe).FirstOrDefaultAsync(x => x.MealRecipeID == id);
            if (mr == null) return null;

            return new MealRecipeResponseDto
            {
                MealRecipeID = mr.MealRecipeID,
                MealEntryID = mr.MealEntryID,
                RecipeID = mr.RecipeID,
                RecipeTitle = mr.Recipe?.Title,
                Quantity = mr.Quantity
            };
        }

        public async Task<MealRecipeResponseDto> Create(MealRecipeCreateDto dto)
        {
            var entity = new MealRecipes
            {
                MealRecipeID = Guid.NewGuid().ToString(),
                MealEntryID = dto.MealEntryID,
                RecipeID = dto.RecipeID,
                Quantity = dto.Quantity
            };

            _context.MealRecipes.Add(entity);
            await _context.SaveChangesAsync();
            await RecalculateMealEntryNutrition(dto.MealEntryID);

            return await GetById(entity.MealRecipeID) ?? throw new Exception("Created entity not found");
        }

        public async Task<MealRecipeResponseDto?> Update(string id, MealRecipeCreateDto dto)
        {
            var existing = await _context.MealRecipes.FindAsync(id);
            if (existing == null) return null;

            existing.MealEntryID = dto.MealEntryID;
            existing.RecipeID = dto.RecipeID;
            existing.Quantity = dto.Quantity;

            await _context.SaveChangesAsync();
            await RecalculateMealEntryNutrition(dto.MealEntryID);

            return await GetById(id);
        }

        public async Task<bool> Delete(string id)
        {
            var entity = await _context.MealRecipes.FindAsync(id);
            if (entity == null) return false;

            var entryId = entity.MealEntryID;

            _context.MealRecipes.Remove(entity);
            await _context.SaveChangesAsync();
            await RecalculateMealEntryNutrition(entryId);
            return true;
        }

        private async Task RecalculateMealEntryNutrition(string mealEntryId)
        {
            var entry = await _context.MealEntries.FindAsync(mealEntryId);
            if (entry == null) return;

            var mealFoods = await _context.MealFoods.Include(mf => mf.Food).Where(mf => mf.MealEntryID == mealEntryId).ToListAsync();
            var mealRecipes = await _context.MealRecipes.Include(mr => mr.Recipe).Where(mr => mr.MealEntryID == mealEntryId).ToListAsync();

            entry.SumProtein = mealFoods.Sum(mf => mf.Quantity / 100f * mf.Food.Protein) + mealRecipes.Sum(mr => mr.Quantity * mr.Recipe.SumProtein);
            entry.SumCarb = mealFoods.Sum(mf => mf.Quantity / 100f * mf.Food.Carb) + mealRecipes.Sum(mr => mr.Quantity * mr.Recipe.SumCarb);
            entry.SumFat = mealFoods.Sum(mf => mf.Quantity / 100f * mf.Food.Fat) + mealRecipes.Sum(mr => mr.Quantity * mr.Recipe.SumFat);
            entry.SumCalorie = mealFoods.Sum(mf => mf.Quantity / 100f * mf.Food.Calorie) + mealRecipes.Sum(mr => mr.Quantity * mr.Recipe.SumCalorie);

            _context.MealEntries.Update(entry);
            await _context.SaveChangesAsync();
        }
    }
}
