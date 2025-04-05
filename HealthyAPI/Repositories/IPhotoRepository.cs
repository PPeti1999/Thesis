using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Repositories
{
    public interface IPhotoRepository
    {
        Task<Photo> GetPhoto(string id);
        Task<Photo> UploadPhoto(Photo photo);
        Task<Photo> UpdatePhoto(Photo photo);
        Task<bool> DeletePhoto(string id);
    }
}
