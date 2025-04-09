using HealthyAPI.Data;
using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace HealthyAPI.Services
{
    public class MealEntriesService : IMealEntriesService
    {
        private readonly Context _context;

        public MealEntriesService(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<MealEntries>> GetAllMealEntries()
        {
            return await _context.MealEntries
                .Include(me => me.MealType)
                .Include(me => me.DailyNote)
                .ToListAsync();
        }

        public async Task<MealEntries?> GetById(string id)
        {
            return await _context.MealEntries
                .Include(me => me.MealType)
                .Include(me => me.DailyNote)
                .FirstOrDefaultAsync(me => me.MealEntryID == id);
        }

        public async Task<MealEntries> Create(MealEntries mealEntry)
        {
            _context.MealEntries.Add(mealEntry);
            await _context.SaveChangesAsync();
            return await GetById(mealEntry.MealEntryID); // visszatöltve include-olt adatokkal
        }

        public async Task<MealEntries?> Update(string id, MealEntries updated)
        {
            var existing = await _context.MealEntries.FindAsync(id);
            if (existing == null) return null;

            existing.DailyNoteID = updated.DailyNoteID;
            existing.MealTypeID = updated.MealTypeID;

            _context.MealEntries.Update(existing);
            await _context.SaveChangesAsync();
            return await GetById(id);
        }

        public async Task<bool> Delete(string id)
        {
            var entry = await _context.MealEntries.FindAsync(id);
            if (entry == null) return false;

            _context.MealEntries.Remove(entry);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
