using HealthyAPI.Data;
using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace HealthyAPI.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Context _context;

        public PhotoService(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Photo>> GetAllPhotos()
        {
            return await _context.Photo.ToListAsync();
        }

        public async Task<Photo?> GetPhoto(string id)
        {
            return await _context.Photo.FindAsync(id);
        }

        public async Task<Photo> UploadPhoto(Photo photo)
        {
            photo.PhotoID = Guid.NewGuid().ToString();
            _context.Photo.Add(photo);
            await _context.SaveChangesAsync();
            return photo;
        }

        public async Task<Photo?> UpdatePhoto(string id, string photoData)
        {
            var existing = await _context.Photo.FindAsync(id);
            if (existing == null) return null;

            existing.PhotoData = photoData;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeletePhoto(string id)
        {
            var entity = await _context.Photo.FindAsync(id);
            if (entity == null) return false;

            _context.Photo.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
