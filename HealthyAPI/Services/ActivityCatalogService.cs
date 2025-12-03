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
            var entities = await _context.ActivityCatalog.ToListAsync();

     
            return entities.Select(f => new ActivityCatalogResponseDto
            {
              ActivityCatalogID = f.ActivityCatalogID,
              Name = f.Name,
              Minute = f.Minute,
              Calories = f.Calories,
              CreatedAt = f.CreatedAt
            });
         }

        public async Task<ActivityCatalogResponseDto> GetById(string id)
        {

          var entity = await _context.ActivityCatalog.FindAsync(id);
          if (entity == null) return null;
          return new ActivityCatalogResponseDto
          {
            ActivityCatalogID = entity.ActivityCatalogID,
            Name = entity.Name,
            Minute = entity.Minute,
            Calories = entity.Calories,
            CreatedAt = entity.CreatedAt
          };
        }

        public async Task<ActivityCatalogResponseDto> Create(ActivityCatalogCreateDto dto)
        {

          var activityCatalog = new ActivityCatalog
          {
            Name = dto.Name,
            Minute = dto.Minute,
            Calories = dto.Calories,
            CreatedAt = DateTime.UtcNow
          };

          _context.ActivityCatalog.Add(activityCatalog);
          await _context.SaveChangesAsync();
          return new ActivityCatalogResponseDto
          {
            ActivityCatalogID = activityCatalog.ActivityCatalogID,
            Name = activityCatalog.Name,
            Minute = activityCatalog.Minute,
            Calories = activityCatalog.Calories,
            CreatedAt = activityCatalog.CreatedAt
          };
        }


        public async Task<ActivityCatalogResponseDto?> Update(string id, ActivityCatalogCreateDto dto)
        {
          var existing = await _context.ActivityCatalog.FindAsync(id);
          if (existing == null) return null;

          existing.Name = dto.Name;
          existing.Minute = dto.Minute;
          existing.Calories = dto.Calories;

          _context.ActivityCatalog.Update(existing);
          await _context.SaveChangesAsync();

          return new ActivityCatalogResponseDto
          {
            ActivityCatalogID = existing.ActivityCatalogID,
            Name = existing.Name,
            Minute = existing.Minute,
            Calories = existing.Calories,
            CreatedAt = existing.CreatedAt
          };
        }

        public async Task<bool> Delete(string id)
        {
            bool hasDependencies = await _context.UserActivity.AnyAsync(dn => dn.ActivityCatalogID == id);
            if (hasDependencies)
                throw new InvalidOperationException("Has dependency");

            var entity = await _context.ActivityCatalog.FindAsync(id);
            if (entity == null) return false;

            _context.ActivityCatalog.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
