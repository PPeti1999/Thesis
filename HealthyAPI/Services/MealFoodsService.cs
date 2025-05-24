using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using HealthyAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using HealthyAPI.DTOs.MealFoods;

namespace HealthyAPI.Services
{
    public class MealFoodsService : IMealFoodsService
    {
        private readonly Context _context;
        private readonly IDailyNoteService _dailyNoteService;

        public MealFoodsService(Context context, IDailyNoteService dailyNoteService)
        {
            _context = context;
            _dailyNoteService = dailyNoteService;
        }

        public async Task<IEnumerable<MealFoodResponseDto>> GetByMealEntryIdAsync(string mealEntryId)
        {
            return await GetMealFoodsByMealEntryId(mealEntryId);
        }

        public async Task<IEnumerable<MealFoodResponseDto>> GetAllMealFoods()
        {
            var entities = await _context.MealFoods.Include(mf => mf.Food).ToListAsync();
            return entities.Select(MapToDto);
        }

        public async Task<MealFoodResponseDto?> GetByIdMealFoods(string id)
        {
            var mf = await _context.MealFoods.Include(mf => mf.Food).FirstOrDefaultAsync(mf => mf.MealFoodID == id);
            return mf == null ? null : MapToDto(mf);
        }

        public async Task<IEnumerable<MealFoodResponseDto>> GetMealFoodsByMealEntryId(string mealEntryId)
        {
            var entities = await _context.MealFoods.Include(mf => mf.Food)
                .Where(mf => mf.MealEntryID == mealEntryId).ToListAsync();
            return entities.Select(MapToDto);
        }

        public async Task<MealFoodResponseDto> CreateMealFoods(MealFoodCreateDto dto)
        {
            var entity = new MealFoods
            {
                MealFoodID = Guid.NewGuid().ToString(),
                MealEntryID = dto.MealEntryID,
                FoodID = dto.FoodID,
                Quantity = dto.Quantity
            };

            _context.MealFoods.Add(entity);
            await _context.SaveChangesAsync();
            await RecalculateMealEntryNutrition(entity.MealEntryID);

            // újra lekérjük includolt Food-dal
            var result = await _context.MealFoods
                .Include(mf => mf.Food)
                .FirstOrDefaultAsync(mf => mf.MealFoodID == entity.MealFoodID);

            return new MealFoodResponseDto
            {
                MealFoodID = result.MealFoodID,
                MealEntryID = result.MealEntryID,
                FoodID = result.FoodID,
                FoodName = result.Food?.Title,
                Quantity = result.Quantity
            };
        }



        public async Task<MealFoodResponseDto?> UpdateMealFoods(string id, MealFoodCreateDto dto)
        {
            var entity = await _context.MealFoods.FindAsync(id);
            if (entity == null) return null;

            entity.FoodID = dto.FoodID;
            entity.MealEntryID = dto.MealEntryID;
            entity.Quantity = dto.Quantity;

            await _context.SaveChangesAsync();
            await RecalculateMealEntryNutrition(entity.MealEntryID);

            // újra lekérjük includolt Food-dal
            var result = await _context.MealFoods
                .Include(mf => mf.Food)
                .FirstOrDefaultAsync(mf => mf.MealFoodID == entity.MealFoodID);

            return new MealFoodResponseDto
            {
                MealFoodID = result.MealFoodID,
                MealEntryID = result.MealEntryID,
                FoodID = result.FoodID,
                FoodName = result.Food?.Title,
                Quantity = result.Quantity
            };
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
            await _dailyNoteService.UpdateMealNutritionAsync(entry.DailyNoteID);
        }

        private MealFoodResponseDto MapToDto(MealFoods mf)
        {
            return new MealFoodResponseDto
            {
                MealFoodID = mf.MealFoodID,
                MealEntryID = mf.MealEntryID,
                FoodID = mf.FoodID,
                FoodName = mf.Food?.Title,
                Quantity = mf.Quantity
            };
        }
    }

}
