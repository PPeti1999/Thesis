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
            _context = context;
        }

        public async Task<IEnumerable<MealFoods>> GetAllMealFoods()
        {
            return await _context.MealFoods.Include(mf => mf.Food).ToListAsync();
        }

        public async Task<MealFoods?> GetByIdMealFoods(string id)
        {
            return await _context.MealFoods.Include(mf => mf.Food).FirstOrDefaultAsync(mf => mf.MealFoodID == id);
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
            return mealFood;
        }

        public async Task<MealFoods?> UpdateMealFoods(string id, MealFoods updated)
        {
            var entity = await _context.MealFoods.FindAsync(id);
            if (entity == null) return null;

            entity.FoodID = updated.FoodID;
            entity.MealEntryID = updated.MealEntryID;
            entity.Quantity = updated.Quantity;

            await _context.SaveChangesAsync();
            await RecalculateMealEntryNutrition(entity.MealEntryID);
            return entity;
        }

        public async Task<bool> DeleteMealFoods(string id)
        {
            var entity = await _context.MealFoods.FindAsync(id);
            if (entity == null) return false;

            var mealEntryId = entity.MealEntryID;

            _context.MealFoods.Remove(entity);
            await _context.SaveChangesAsync();
            await RecalculateMealEntryNutrition(mealEntryId);
            return true;
        }

        public async Task RecalculateMealEntryNutrition(string mealEntryId)
        {
            var entry = await _context.MealEntries.FindAsync(mealEntryId);
            if (entry == null) return;

            var foods = await _context.MealFoods.Include(mf => mf.Food).Where(mf => mf.MealEntryID == mealEntryId).ToListAsync();
            var recipes = await _context.MealRecipes.Include(mr => mr.Recipe).Where(mr => mr.MealEntryID == mealEntryId).ToListAsync();

            entry.SumProtein = foods.Sum(mf => mf.Quantity / 100f * mf.Food.Protein) + recipes.Sum(mr => mr.Quantity * mr.Recipe.SumProtein);
            entry.SumCarb = foods.Sum(mf => mf.Quantity / 100f * mf.Food.Carb) + recipes.Sum(mr => mr.Quantity * mr.Recipe.SumCarb);
            entry.SumFat = foods.Sum(mf => mf.Quantity / 100f * mf.Food.Fat) + recipes.Sum(mr => mr.Quantity * mr.Recipe.SumFat);
            entry.SumCalorie = foods.Sum(mf => mf.Quantity / 100f * mf.Food.Calorie) + recipes.Sum(mr => mr.Quantity * mr.Recipe.SumCalorie);

            _context.MealEntries.Update(entry);
            await _context.SaveChangesAsync();
        }
    }

}
