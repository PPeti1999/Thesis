using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IPhotoService
    {
        Task<IEnumerable<Photo>> GetAllPhotos();
        Task<Photo?> GetPhoto(string id);
        Task<Photo> UploadPhoto(Photo photo);
        Task<Photo?> UpdatePhoto(string id, string photoData);
        Task<bool> DeletePhoto(string id);
    }
}
