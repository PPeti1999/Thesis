using HealthyAPI.Data;
using HealthyAPI.DTOs.ActivityCatalog;
using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace HealthyAPI.Services
{
    public class ActivityCatalogService : IActivityCatalogService
    {
        private readonly Context _context;

        public ActivityCatalogService(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActivityCatalogResponseDto>> GetAll()
        {
            return await _context.ActivityCatalog.Select(a => new ActivityCatalogResponseDto
            {
                ActivityCatalogID = a.ActivityCatalogID,
                Name = a.Name,
                Minute = a.Minute,
                Calories = a.Calories,
                CreatedAt = a.CreatedAt
            }).ToListAsync();
        }

        public async Task<ActivityCatalogResponseDto?> GetById(string id)
        {
            var activity = await _context.ActivityCatalog.FindAsync(id);
            if (activity == null) return null;

            return new ActivityCatalogResponseDto
            {
                ActivityCatalogID = activity.ActivityCatalogID,
                Name = activity.Name,
                Minute = activity.Minute,
                Calories = activity.Calories,
                CreatedAt = activity.CreatedAt
            };
        }

        public async Task<ActivityCatalogResponseDto> Create(ActivityCatalogCreateDto dto)
        {
            var entity = new ActivityCatalog
            {
                ActivityCatalogID = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Minute = dto.Minute,
                Calories = dto.Calories,
                CreatedAt = DateTime.UtcNow
            };

            _context.ActivityCatalog.Add(entity);
            await _context.SaveChangesAsync();

            return await GetById(entity.ActivityCatalogID) ?? throw new Exception("Creation failed");
        }

        public async Task<ActivityCatalogResponseDto?> Update(string id, ActivityCatalogCreateDto dto)
        {
            var entity = await _context.ActivityCatalog.FindAsync(id);
            if (entity == null) return null;

            entity.Name = dto.Name;
            entity.Minute = dto.Minute;
            entity.Calories = dto.Calories;

            await _context.SaveChangesAsync();
            return await GetById(id);
        }

        public async Task<bool> Delete(string id)
        {
            var entity = await _context.ActivityCatalog.FindAsync(id);
            if (entity == null) return false;

            _context.ActivityCatalog.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
