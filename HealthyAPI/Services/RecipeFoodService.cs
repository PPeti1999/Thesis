using HealthyAPI.Data;
using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace HealthyAPI.Services
{
    public class RecipeFoodService : IRecipeFoodService
    {
        private readonly Context _context;

        public RecipeFoodService(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RecipeFoods>> GetAll()
        {
            return await _context.RecipeFoods.Include(rf => rf.Food).ToListAsync();
        }

        public async Task<RecipeFoods?> GetById(string id)
        {
            return await _context.RecipeFoods.Include(rf => rf.Food).FirstOrDefaultAsync(rf => rf.RecipeFoodID == id);
        }

        public async Task<RecipeFoods> Create(RecipeFoods entity)
        {
            entity.RecipeFoodID = Guid.NewGuid().ToString();
            _context.RecipeFoods.Add(entity);
            await _context.SaveChangesAsync();

            return await _context.RecipeFoods
                .Include(rf => rf.Food)
                .FirstOrDefaultAsync(rf => rf.RecipeFoodID == entity.RecipeFoodID);
        }

        public async Task<RecipeFoods?> Update(string id, RecipeFoods updated)
        {
            var entity = await _context.RecipeFoods.FindAsync(id);
            if (entity == null) return null;

            entity.FoodID = updated.FoodID;
            entity.RecipeID = updated.RecipeID;
            entity.Quantity = updated.Quantity;

            await _context.SaveChangesAsync();
            return await _context.RecipeFoods.Include(rf => rf.Food).FirstOrDefaultAsync(rf => rf.RecipeFoodID == id);
        }

        public async Task<bool> Delete(string id)
        {
            var entity = await _context.RecipeFoods.FindAsync(id);
            if (entity == null) return false;

            _context.RecipeFoods.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
