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
        private readonly IUserActivityService _iuseractivityservice;

        public ActivityCatalogService(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActivityCatalog>> GetAll()
        {
            return await _context.ActivityCatalog.ToListAsync();
        }

        public async Task<ActivityCatalog?> GetById(string id)
        {

            return await _context.ActivityCatalog.FindAsync(id);
        }

        public async Task<ActivityCatalog> Create(ActivityCatalog dto)
        {


            _context.ActivityCatalog.Add(dto);
            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<ActivityCatalog?> Update(string id, ActivityCatalog dto)
        {
            var existing = await _context.ActivityCatalog.FindAsync(id);
            if (existing == null) return null;

            _context.ActivityCatalog.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> Delete(string id)
        {
            // Ellenőrzés: van-e kapcsolódó bejegyzés
            bool hasDependencies = await _context.UserActivity.AnyAsync(dn => dn.ActivityCatalogID == id);
            if (hasDependencies)
                throw new InvalidOperationException("Az aktivitás már használatban van, nem törölhető.");

            var entity = await _context.ActivityCatalog.FindAsync(id);
            if (entity == null) return false;

            _context.ActivityCatalog.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
