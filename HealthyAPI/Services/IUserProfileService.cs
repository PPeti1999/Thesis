using HealthyAPI.DTOs.Profile;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IUserProfileService
    {
        Task<UserProfileResponseDto?> GetCurrentUserProfile(string userId);
        Task<UserProfileResponseDto?> UpdateProfile(string userId, UpdateUserProfileDto dto);
    }
}
