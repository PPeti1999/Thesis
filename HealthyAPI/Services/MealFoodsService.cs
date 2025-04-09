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
                return await _context.MealFoods
             .Include(mf => mf.Food)
             .Include(mf => mf.MealEntry) // opcionális
             .ToListAsync();
        }

            public async Task<MealFoods?> GetByIdMealFoods(string id)
            {
                return await _context.MealFoods
             .Include(mf => mf.Food)
             .Include(mf => mf.MealEntry)
             .FirstOrDefaultAsync(mf => mf.MealFoodID == id);
        }
        public async Task<IEnumerable<MealFoods>> GetMealFoodsByMealEntryId(string mealEntryId)
        {
            return await _context.MealFoods
                .Where(mf => mf.MealEntryID == mealEntryId)
                .Include(mf => mf.Food)
                .ToListAsync();
        }

        public async Task<MealFoods> CreateMealFoods(MealFoods mealFood)
            {
                _context.MealFoods.Add(mealFood);
                await _context.SaveChangesAsync();
            return await _context.MealFoods
             .Include(mf => mf.Food)
             .Include(mf => mf.MealEntry)
                 .ThenInclude(me => me.MealType)
             .Include(mf => mf.MealEntry)
                 .ThenInclude(me => me.DailyNote)
             .FirstOrDefaultAsync(mf => mf.MealFoodID == mealFood.MealFoodID);
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
                return existing;
            }

            public async Task<bool> DeleteMealFoods(string id)
            {
                var entity = await _context.MealFoods.FindAsync(id);
                if (entity == null) return false;

                _context.MealFoods.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
        }
    
}
