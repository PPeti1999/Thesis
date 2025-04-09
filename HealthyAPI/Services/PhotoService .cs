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
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Photo> GetPhoto(string id)
        {
            return await _context.Photo.FirstOrDefaultAsync(p => p.PhotoID == id);
        }

        public async Task<Photo> UploadPhoto(Photo photo)
        {
            _context.Photo.Add(photo);
            await _context.SaveChangesAsync();
            return photo;
        }

        public async Task<Photo> UpdatePhoto(Photo photo)
        {
            _context.Photo.Update(photo);
            await _context.SaveChangesAsync();
            return photo;
        }

        public async Task<bool> DeletePhoto(string id)
        {
            var photo = await _context.Photo.FindAsync(id);
            if (photo == null)
                return false;
            _context.Photo.Remove(photo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
