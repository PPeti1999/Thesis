using HealthyAPI.Data;
using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace HealthyAPI.Services
{
    public class MealTypeService : IMealTypeService
    {
        private readonly Context _context;

        public MealTypeService(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MealTypes>> GetAll()
        {
            return await _context.MealTypes.Include(mt => mt.Photo).ToListAsync();
        }

        public async Task<MealTypes?> GetById(string id)
        {
            return await _context.MealTypes.Include(mt => mt.Photo).FirstOrDefaultAsync(mt => mt.MealTypeID == id);
        }

        public async Task<MealTypes> Create(MealTypes entity)
        {
            entity.MealTypeID = Guid.NewGuid().ToString();
            _context.MealTypes.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<MealTypes?> Update(string id, MealTypes updated)
        {
            var entity = await _context.MealTypes.FindAsync(id);
            if (entity == null) return null;

            entity.Name = updated.Name;
            entity.PhotoID = updated.PhotoID;

            await _context.SaveChangesAsync();

            return await _context.MealTypes
                .Include(mt => mt.Photo)
                .FirstOrDefaultAsync(mt => mt.MealTypeID == id);
        }

        public async Task<bool> Delete(string id)
        {
            var entity = await _context.MealTypes.FindAsync(id);
            if (entity == null) return false;

            bool hasEntries = await _context.MealEntries.AnyAsync(me => me.MealTypeID == id);
            if (hasEntries)
                throw new InvalidOperationException("A törlés nem lehetséges: van olyan étkezés, amely ezt a típust használja.");

            _context.MealTypes.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
