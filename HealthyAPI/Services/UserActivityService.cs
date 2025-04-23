using HealthyAPI.Data;
using HealthyAPI.DTOs.UserActivity;
using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HealthyAPI.Services
{
    public class UserActivityService : IUserActivityService
    {
        private readonly Context _context;

        public UserActivityService(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserActivityResponseDto>> GetAll()
        {
            var activities = await _context.UserActivity
                .Include(ua => ua.ActivityCatalog)
                .Include(ua => ua.Photo)
                .ToListAsync();

            return activities.Select(ua => new UserActivityResponseDto
            {
                UserActivityID = ua.UserActivityID,
                DailyNoteID = ua.DailyNoteID,
                ActivityCatalogID = ua.ActivityCatalogID ?? string.Empty,
                ActivityName = ua.ActivityCatalog?.Name ?? string.Empty,
                Duration = ua.Duration,
                Calories = ua.Calories,
                PhotoID = ua.PhotoID ?? string.Empty,
                PhotoData = ua.Photo?.PhotoData ?? string.Empty
            });
        }

        public async Task<UserActivityResponseDto?> GetById(string id)
        {
            var entity = await _context.UserActivity
                .Include(ua => ua.ActivityCatalog)
                .Include(ua => ua.Photo)
                .FirstOrDefaultAsync(ua => ua.UserActivityID == id);

            if (entity == null) return null;

            return new UserActivityResponseDto
            {
                UserActivityID = entity.UserActivityID,
                DailyNoteID = entity.DailyNoteID,
                ActivityCatalogID = entity.ActivityCatalogID ?? string.Empty,
                ActivityName = entity.ActivityCatalog?.Name ?? string.Empty,
                Duration = entity.Duration,
                Calories = entity.Calories,
                PhotoID = entity.PhotoID ?? string.Empty,
                PhotoData = entity.Photo?.PhotoData ?? string.Empty
            };
        }

        public async Task<UserActivityResponseDto> Create(UserActivityCreateDto dto)
        {
            var catalog = await _context.ActivityCatalog.FindAsync(dto.ActivityCatalogID);
            if (catalog == null) throw new ArgumentException("Invalid activity catalog ID");

            var calories = (int)(dto.Duration / (float)catalog.Minute * catalog.Calories);

            var entity = new UserActivity
            {
                UserActivityID = Guid.NewGuid().ToString(),
                DailyNoteID = dto.DailyNoteID,
                ActivityCatalogID = dto.ActivityCatalogID,
                Duration = dto.Duration,
                Calories = calories,
                PhotoID = dto.PhotoID
            };

            _context.UserActivity.Add(entity);

            // 🔥 Itt frissítjük a DailyNote célkalória értékét
            var dailyNote = await _context.DailyNote.FindAsync(dto.DailyNoteID);
            if (dailyNote != null)
            {
                dailyNote.DailyTargetCalorie += calories;
            }

            await _context.SaveChangesAsync();

            return await GetById(entity.UserActivityID) ?? throw new Exception("Entity not found after creation.");
        }

        public async Task<UserActivityResponseDto?> Update(string id, UserActivityCreateDto dto)
        {
            var entity = await _context.UserActivity.Include(ua => ua.ActivityCatalog).FirstOrDefaultAsync(ua => ua.UserActivityID == id);
            if (entity == null) return null;

            var catalog = await _context.ActivityCatalog.FindAsync(dto.ActivityCatalogID);
            if (catalog == null) throw new ArgumentException("Invalid activity catalog ID");

            int oldCalories = entity.Calories;
            int newCalories = (int)(dto.Duration / (float)catalog.Minute * catalog.Calories);
            int deltaCalories = newCalories - oldCalories;

            entity.DailyNoteID = dto.DailyNoteID;
            entity.ActivityCatalogID = dto.ActivityCatalogID;
            entity.Duration = dto.Duration;
            entity.Calories = newCalories;
            entity.PhotoID = dto.PhotoID;

            // 🔥 Itt frissítjük a DailyNote célkalória értékét a különbséggel
            var dailyNote = await _context.DailyNote.FindAsync(dto.DailyNoteID);
            if (dailyNote != null)
            {
                dailyNote.DailyTargetCalorie += deltaCalories;
            }

            await _context.SaveChangesAsync();
            return await GetById(id);
        }

        public async Task<bool> Delete(string id)
        {
            var entity = await _context.UserActivity.FindAsync(id);
            if (entity == null) return false;

            _context.UserActivity.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
