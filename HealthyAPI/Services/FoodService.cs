using HealthyAPI.Data;
using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace HealthyAPI.Services
{
    public class FoodService : IFoodService
    {
        private readonly Context _context;

        public FoodService(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Food>> ListFoods()
        {
            return await _context.Food
                .Include(f => f.Photo)
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

        public async Task<Food> UpdateFood(string id, Food food)
        {
            var existing = await _context.Food.FindAsync(id);
            if (existing == null) return null;

            _context.Food.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
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

