using HealthyAPI.Data;
using HealthyAPI.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<MealRecipes>> GetAll()
        {
            return await _context.MealRecipes.Include(mr => mr.Recipe).ToListAsync();
        }

        public async Task<MealRecipes?> GetById(string id)
        {
            return await _context.MealRecipes.Include(mr => mr.Recipe).FirstOrDefaultAsync(mr => mr.MealRecipeID == id);
        }

        public async Task<MealRecipes> Create(MealRecipes entity)
        {
            _context.MealRecipes.Add(entity);
            await _context.SaveChangesAsync();
            await RecalculateMealEntryNutrition(entity.MealEntryID);
            return entity;
        }

        public async Task<MealRecipes?> Update(string id, MealRecipes entity)
        {
            var existing = await _context.MealRecipes.FindAsync(id);
            if (existing == null) return null;

            existing.MealEntryID = entity.MealEntryID;
            existing.RecipeID = entity.RecipeID;
            existing.Quantity = entity.Quantity;

            await _context.SaveChangesAsync();
            await RecalculateMealEntryNutrition(entity.MealEntryID);
            return existing;
        }

        public async Task<bool> Delete(string id)
        {
            var entity = await _context.MealRecipes.FindAsync(id);
            if (entity == null) return false;
            string mealEntryId = entity.MealEntryID;

            _context.MealRecipes.Remove(entity);
            await _context.SaveChangesAsync();
            await RecalculateMealEntryNutrition(mealEntryId);
            return true;
        }

        public async Task RecalculateMealEntryNutrition(string mealEntryId)
        {
            var entry = await _context.MealEntries.FindAsync(mealEntryId);
            if (entry == null) return;

            var mealFoods = await _context.MealFoods.Include(mf => mf.Food)
                .Where(mf => mf.MealEntryID == mealEntryId).ToListAsync();
            var mealRecipes = await _context.MealRecipes.Include(mr => mr.Recipe)
                .Where(mr => mr.MealEntryID == mealEntryId).ToListAsync();

            entry.SumProtein = mealFoods.Sum(mf => mf.Quantity / 100f * mf.Food.Protein)
                                + mealRecipes.Sum(mr => mr.Quantity * mr.Recipe.SumProtein);

            entry.SumCarb = mealFoods.Sum(mf => mf.Quantity / 100f * mf.Food.Carb)
                             + mealRecipes.Sum(mr => mr.Quantity * mr.Recipe.SumCarb);

            entry.SumFat = mealFoods.Sum(mf => mf.Quantity / 100f * mf.Food.Fat)
                            + mealRecipes.Sum(mr => mr.Quantity * mr.Recipe.SumFat);

            entry.SumCalorie = mealFoods.Sum(mf => mf.Quantity / 100f * mf.Food.Calorie)
                                + mealRecipes.Sum(mr => mr.Quantity * mr.Recipe.SumCalorie);

            _context.MealEntries.Update(entry);
            await _context.SaveChangesAsync();
        }
    }
}
