using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using HealthyAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HealthyAPI.Services
{
    public class MealFoodsService : IMealFoodsService
    {
        private readonly Context _context;

        public MealFoodsService(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<MealFoods>> GetAllMealFoods()
        {
            return await _context.MealFoods.Include(mf => mf.Food).Include(mf => mf.MealEntry).ToListAsync();
        }

        public async Task<MealFoods?> GetByIdMealFoods(string id)
        {
            return await _context.MealFoods.Include(mf => mf.Food).Include(mf => mf.MealEntry).FirstOrDefaultAsync(mf => mf.MealFoodID == id);
        }

        public async Task<IEnumerable<MealFoods>> GetMealFoodsByMealEntryId(string mealEntryId)
        {
            return await _context.MealFoods.Include(mf => mf.Food).Where(mf => mf.MealEntryID == mealEntryId).ToListAsync();
        }

        public async Task<MealFoods> CreateMealFoods(MealFoods mealFood)
        {
            _context.MealFoods.Add(mealFood);
            await _context.SaveChangesAsync();
            await RecalculateMealEntryNutrition(mealFood.MealEntryID);
            return await GetByIdMealFoods(mealFood.MealFoodID);
        }

        public async Task<MealFoods> UpdateMealFoods(string id, MealFoods updated)
        {
            var existing = await _context.MealFoods.FindAsync(id);
            if (existing == null) return null;

            existing.FoodID = updated.FoodID;
            existing.MealEntryID = updated.MealEntryID;
            existing.Quantity = updated.Quantity;

            _context.MealFoods.Update(existing);
            await _context.SaveChangesAsync();
            await RecalculateMealEntryNutrition(updated.MealEntryID);
            return await GetByIdMealFoods(id);
        }

        public async Task<bool> DeleteMealFoods(string id)
        {
            var entity = await _context.MealFoods.FindAsync(id);
            if (entity == null) return false;
            string mealEntryId = entity.MealEntryID;

            _context.MealFoods.Remove(entity);
            await _context.SaveChangesAsync();
            await RecalculateMealEntryNutrition(mealEntryId);
            return true;
        }

        public async Task RecalculateMealEntryNutrition(string mealEntryId)
        {
            var entry = await _context.MealEntries.FindAsync(mealEntryId);
            if (entry == null) return;

            var mealFoods = await _context.MealFoods.Include(mf => mf.Food).Where(mf => mf.MealEntryID == mealEntryId).ToListAsync();
            var mealRecipes = await _context.MealRecipes.Include(mr => mr.Recipe).Where(mr => mr.MealEntryID == mealEntryId).ToListAsync();

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
