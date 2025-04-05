using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using HealthyAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace HealthyAPI.Repositories
{
    public class FoodRepository : IFoodRepository
    {
        private readonly Context _context;

        public FoodRepository(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Food>> ListFoods()
        {
            return await _context.Food
                .Include(f => f.Photo)  // Kapcsolódó fotó betöltése
                .ToListAsync();
        }

        public async Task<Food> GetFood(string id)
        {
            return await _context.Food
                .Include(f => f.Photo)
                .FirstOrDefaultAsync(f => f.FoodID == id);
        }

        public async Task<Food> AddFood(Food food)
        {
            _context.Food.Add(food);
            await _context.SaveChangesAsync();
            return food;
        }

        public async Task<Food> UpdateFood(Food food)
        {
            if (food == null)
            {
                return food;
            }
            _context.Food.Update(food);
            await _context.SaveChangesAsync();
            return food;
        }

        public async Task<bool> DeleteFood(string id)
        {
            var food = await _context.Food.FindAsync(id);
            if (food == null)
                return false;

            _context.Food.Remove(food);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
