using HealthyAPI.Data;
using HealthyAPI.DTOs.DailyNote;
using HealthyAPI.Models;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HealthyAPI.Services
{
    namespace HealthyAPI.Services
    {
        public class DailyNoteService : IDailyNoteService
        {
            private readonly Context _context;

            public DailyNoteService(Context context)
            {
                _context = context;
            }

            public async Task<DailyNoteResponseDto?> GetTodayNote(string userId)
            {
                var today = DateTime.Today;
                var note = await _context.DailyNote.FirstOrDefaultAsync(d => d.UserID == userId && d.CreatedAt.Date == today);
                if (note == null) return null;

                return MapToResponse(note);
            }

            public async Task<DailyNoteResponseDto> CreateDailyNote(string userId)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) throw new Exception("Felhasználó nem található.");

                var today = DateTime.Today;
                var exists = await _context.DailyNote.AnyAsync(d => d.UserID == userId && d.CreatedAt.Date == today);
                if (exists) throw new Exception("Már van DailyNote a mai napra.");

                var yesterday = await _context.DailyNote
                    .Where(d => d.UserID == userId && d.CreatedAt.Date < today)
                    .OrderByDescending(d => d.CreatedAt)
                    .FirstOrDefaultAsync();

                int startingWeight = yesterday?.DailyWeight ?? user.Weight;

                var note = new DailyNote
                {
                    DailyNoteID = Guid.NewGuid().ToString(),
                    UserID = user.Id,
                    CreatedAt = today,
                    DailyWeight = startingWeight,
                    DailyTargetCalorie = user.TargetCalorie,
                    DailyTargetProtein = user.TargeProtein,
                    DailyTargetCarb = user.TargetCarb,
                    DailyTargetFat = user.TargetFat,
                    ActualCalorie = 0,
                    ActualSumCarb = 0,
                    ActualSumFat = 0,
                    ActualSumProtein = 0
                };

                _context.DailyNote.Add(note);
                await _context.SaveChangesAsync();

                var mealTypes = await _context.MealTypes.ToListAsync();
                foreach (var mt in mealTypes)
                {
                    _context.MealEntries.Add(new MealEntries
                    {
                        MealEntryID = Guid.NewGuid().ToString(),
                        DailyNoteID = note.DailyNoteID,
                        MealTypeID = mt.MealTypeID
                    });
                }

                await _context.SaveChangesAsync();

                return MapToResponse(note);
            }

            public async Task UpdateMealNutritionAsync(string dailyNoteId)
            {
                var meals = await _context.MealEntries
                    .Where(me => me.DailyNoteID == dailyNoteId)
                    .Include(me => me.MealFoods).ThenInclude(mf => mf.Food)
                    .Include(me => me.MealRecipes).ThenInclude(mr => mr.Recipe)
                    .ToListAsync();

                float protein = 0, fat = 0, carb = 0;
                int calorie = 0;

                foreach (var meal in meals)
                {
                    foreach (var mf in meal.MealFoods)
                    {
                        var factor = mf.Quantity / (float)mf.Food.Gram;
                        protein += mf.Food.Protein * factor;
                        fat += mf.Food.Fat * factor;
                        carb += mf.Food.Carb * factor;
                        calorie += (int)(mf.Food.Calorie * factor);
                    }
                    foreach (var mr in meal.MealRecipes)
                    {
                        protein += mr.Recipe.SumProtein * mr.Quantity;
                        fat += mr.Recipe.SumFat * mr.Quantity;
                        carb += mr.Recipe.SumCarb * mr.Quantity;
                        calorie += (int)(mr.Recipe.SumCalorie * mr.Quantity);
                    }
                }

                var dailyNote = await _context.DailyNote.FindAsync(dailyNoteId);
                if (dailyNote != null)
                {
                    dailyNote.ActualSumProtein = protein;
                    dailyNote.ActualSumFat = fat;
                    dailyNote.ActualSumCarb = carb;
                    dailyNote.ActualCalorie = calorie;
                    await _context.SaveChangesAsync();
                }
            }


            public async Task<DailyNoteResponseDto?> UpdateWeight(string dailyNoteId, int weight)
            {
                var note = await _context.DailyNote.FindAsync(dailyNoteId);
                if (note == null) return null;
                note.DailyWeight = weight;
                await _context.SaveChangesAsync();
                return MapToResponse(note);
            }

            private DailyNoteResponseDto MapToResponse(DailyNote note)
            {
                return new DailyNoteResponseDto
                {
                    DailyNoteID = note.DailyNoteID,
                    UserID = note.UserID,
                    DailyWeight = note.DailyWeight,
                    DailyTargetCalorie = note.DailyTargetCalorie,
                    ActualCalorie = note.ActualCalorie,
                    DailyTargetProtein = note.DailyTargetProtein,
                    ActualSumProtein = note.ActualSumProtein,
                    DailyTargetCarb = note.DailyTargetCarb,
                    ActualSumCarb = note.ActualSumCarb,
                    DailyTargetFat = note.DailyTargetFat,
                    ActualSumFat = note.ActualSumFat,
                    CreatedAt = note.CreatedAt
                };
            }
        }
    }

}
