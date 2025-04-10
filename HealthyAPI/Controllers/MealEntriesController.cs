using HealthyAPI.Data;
using HealthyAPI.DTOs.Food;
using HealthyAPI.DTOs.MealEntries;
using HealthyAPI.Models;
using HealthyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealEntriesController : ControllerBase
    {
        private readonly Context _context;

        public MealEntriesController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MealEntryResponseDto>>> GetAll()
        {
            var entries = await _context.MealEntries.Include(me => me.MealType).ToListAsync();

            var result = entries.Select(e => new MealEntryResponseDto
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

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<MealEntryResponseDto>> GetById(string id)
        {
            var entry = await _context.MealEntries.Include(me => me.MealType)
                .FirstOrDefaultAsync(me => me.MealEntryID == id);

            if (entry == null) return NotFound();

            return Ok(new MealEntryResponseDto
            {
                MealEntryID = entry.MealEntryID,
                DailyNoteID = entry.DailyNoteID,
                MealTypeID = entry.MealTypeID,
                MealTypeName = entry.MealType?.Name,
                SumProtein = entry.SumProtein,
                SumCarb = entry.SumCarb,
                SumFat = entry.SumFat,
                SumCalorie = entry.SumCalorie
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MealEntryResponseDto>> Create(MealEntryCreateDto dto)
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

            var created = await _context.MealEntries.Include(me => me.MealType)
                .FirstOrDefaultAsync(me => me.MealEntryID == entity.MealEntryID);

            return CreatedAtAction(nameof(GetById), new { id = created.MealEntryID }, new MealEntryResponseDto
            {
                MealEntryID = created.MealEntryID,
                DailyNoteID = created.DailyNoteID,
                MealTypeID = created.MealTypeID,
                MealTypeName = created.MealType?.Name,
                SumProtein = created.SumProtein,
                SumCarb = created.SumCarb,
                SumFat = created.SumFat,
                SumCalorie = created.SumCalorie
            });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _context.MealEntries.FindAsync(id);
            if (entity == null) return NotFound();

            _context.MealEntries.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
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

