using HealthyAPI.Data;
using HealthyAPI.DTOs.CalendarSummary;
using HealthyAPI.DTOs.DailyNote;
using HealthyAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


    namespace HealthyAPI.Services
    {
        public class DailyNoteService : IDailyNoteService
        {
        private readonly Context _context;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public DailyNoteService(Context context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetUserId()
        {

            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        public async Task<List<CalendarSummaryDto>> GetMonthlySummaryAsync(string userId, int year, int month)
            {
                var notes = await _context.DailyNote
                    .Where(d => d.UserID == userId &&
                                d.CreatedAt.Year == year &&
                                d.CreatedAt.Month == month)
                    .ToListAsync();

                return notes.Select(d => new CalendarSummaryDto
                {
                    Date = d.CreatedAt.Date,
                    RemainingKcal = (d.DailyTargetCalorie ) - (d.ActualCalorie)
                }).ToList();
            }

            public async Task<DailyNoteResponseDto?> GetPreviousNote(string userId, DateTime currentDate)
            {
                var prev = await _context.DailyNote
                    .Where(d => d.UserID == userId && d.CreatedAt < currentDate)
                    .OrderByDescending(d => d.CreatedAt)
                    .FirstOrDefaultAsync();

                return prev != null ? MapToResponse(prev) : null;
            }



            public async Task<DailyNoteResponseDto?> GetNextNote(string userId, DateTime currentDate)
            {
                

                var next = await _context.DailyNote
                    .Where(d => d.UserID == userId && d.CreatedAt.Date > currentDate)
                    .OrderBy(d => d.CreatedAt)
                    .FirstOrDefaultAsync();

                return next != null ? MapToResponse(next) : null;
            }



            public async Task<DailyNoteResponseDto?> GetNoteByDate(string userId, DateTime date)
            {
                var note = await _context.DailyNote
                    .FirstOrDefaultAsync(d => d.UserID == userId && d.CreatedAt.Date == date.Date);

                return note != null ? MapToResponse(note) : null;
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
                    dailyNote.ActualSumProtein = (float)Math.Round((double)protein, 2);
                  dailyNote.ActualSumFat = (float)Math.Round((double)fat, 2);
                  dailyNote.ActualSumCarb = (float)Math.Round((double)carb, 2);
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
        public async Task<List<DailyNoteResponseDto>> GetAllDailyNotesForGraph()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return new List<DailyNoteResponseDto>(); // Üres listát adunk vissza null helyett

            // Lekérdezzük az összes DailyNote-ot, ahol a DailyWeight > 0, 
            // rendezve dátum szerint, és csak a Dátum és a Súly érdekel.
            var dailyNotes = await _context.DailyNote
                .Where(dn => dn.UserID == userId && dn.DailyWeight > 0)
                .OrderBy(dn => dn.CreatedAt)
                .Select(dn => new DailyNoteResponseDto
                {
                    DailyNoteID = dn.DailyNoteID, // Az Id technikai okokból kell
                    CreatedAt = dn.CreatedAt,
                    DailyWeight = dn.DailyWeight,
                    // A többi mezőt nem kell feltölteni, de a DTO megköveteli
                    UserID = dn.UserID,
                    DailyTargetCalorie = dn.DailyTargetCalorie,
                    ActualCalorie = dn.ActualCalorie,
                    DailyTargetProtein = dn.DailyTargetProtein,
                    ActualSumProtein = dn.ActualSumProtein,
                    DailyTargetCarb = dn.DailyTargetCarb,
                    ActualSumCarb = dn.ActualSumCarb,
                    DailyTargetFat = dn.DailyTargetFat,
                    ActualSumFat = dn.ActualSumFat,
                })
                .ToListAsync();

            return dailyNotes;
        }



    }
}


