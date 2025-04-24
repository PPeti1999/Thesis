using HealthyAPI.Data;
using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using HealthyAPI.DTOs.Food;
using HealthyAPI.DTOs.MealEntries;
using System.Linq;

namespace HealthyAPI.Services
{
    public class MealEntriesService : IMealEntriesService
    {
        private readonly Context _context;

        private readonly IDailyNoteService _dailyNoteService;

        public MealEntriesService(Context context, IDailyNoteService dailyNoteService)
        {
            _context = context;
            _dailyNoteService = dailyNoteService;
        }

        public async Task<IEnumerable<MealEntryResponseDto>> GetAll()
        {
            var entries = await _context.MealEntries.Include(me => me.MealType).ToListAsync();
            return entries.Select(e => new MealEntryResponseDto
            {
                MealEntryID = e.MealEntryID,
                DailyNoteID = e.DailyNoteID,
                MealTypeID = e.MealTypeID,
                MealTypeName = e.MealType?.Name,
                SumProtein = e.SumProtein,
                SumCarb = e.SumCarb,
                SumFat = e.SumFat,
                SumCalorie = e.SumCalorie
            });
        }

        public async Task<MealEntryResponseDto?> GetById(string id)
        {
            var entry = await _context.MealEntries.Include(me => me.MealType)
                .FirstOrDefaultAsync(me => me.MealEntryID == id);
            if (entry == null) return null;

            return new MealEntryResponseDto
            {
                MealEntryID = entry.MealEntryID,
                DailyNoteID = entry.DailyNoteID,
                MealTypeID = entry.MealTypeID,
                MealTypeName = entry.MealType?.Name,
                SumProtein = entry.SumProtein,
                SumCarb = entry.SumCarb,
                SumFat = entry.SumFat,
                SumCalorie = entry.SumCalorie
            };
        }

        public async Task<MealEntryResponseDto> Create(MealEntryCreateDto dto)
        {
            var entity = new MealEntries
            {
                MealEntryID = Guid.NewGuid().ToString(),
                DailyNoteID = dto.DailyNoteID,
                MealTypeID = dto.MealTypeID
            };

            _context.MealEntries.Add(entity);
            await _context.SaveChangesAsync();
            await RecalculateNutrition(entity.MealEntryID);
            await _dailyNoteService.UpdateMealNutritionAsync(entity.DailyNoteID); // vagy id alapján lekérve
            return await GetById(entity.MealEntryID) ?? throw new Exception("Created entry not found");
        }

        public async Task<MealEntryResponseDto?> Update(string id, MealEntryCreateDto dto)
        {
            var entity = await _context.MealEntries.FindAsync(id);
            if (entity == null) return null;

            entity.DailyNoteID = dto.DailyNoteID;
            entity.MealTypeID = dto.MealTypeID;

            await _context.SaveChangesAsync();
            await RecalculateNutrition(id);
            await _dailyNoteService.UpdateMealNutritionAsync(entity.DailyNoteID); // vagy id alapján lekérve
            return await GetById(id);
        }

        public async Task<bool> Delete(string id)
        {
            var entity = await _context.MealEntries.FindAsync(id);
            if (entity == null) return false;
            var dailyNoteId = entity.DailyNoteID;
            _context.MealEntries.Remove(entity);
            await _context.SaveChangesAsync();
            await _dailyNoteService.UpdateMealNutritionAsync(dailyNoteId);
            return true;
        }

        private async Task RecalculateNutrition(string mealEntryId)
        {
            var mealFoods = await _context.MealFoods.Include(mf => mf.Food)
                .Where(mf => mf.MealEntryID == mealEntryId).ToListAsync();

            var mealRecipes = await _context.MealRecipes.Include(mr => mr.Recipe)
                .Where(mr => mr.MealEntryID == mealEntryId).ToListAsync();

            float protein = 0, carb = 0, fat = 0, cal = 0;

            foreach (var mf in mealFoods)
            {
                protein += mf.Quantity / 100f * mf.Food.Protein;
                carb += mf.Quantity / 100f * mf.Food.Carb;
                fat += mf.Quantity / 100f * mf.Food.Fat;
                cal += mf.Quantity / 100f * mf.Food.Calorie;
            }

            foreach (var mr in mealRecipes)
            {
                protein += mr.Quantity * mr.Recipe.SumProtein;
                carb += mr.Quantity * mr.Recipe.SumCarb;
                fat += mr.Quantity * mr.Recipe.SumFat;
                cal += mr.Quantity * mr.Recipe.SumCalorie;
            }

            var entry = await _context.MealEntries.FindAsync(mealEntryId);
            if (entry != null)
            {
                entry.SumProtein = protein;
                entry.SumCarb = carb;
                entry.SumFat = fat;
                entry.SumCalorie = cal;
                await _context.SaveChangesAsync();
            }
        }
    }
}
