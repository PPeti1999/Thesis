using HealthyAPI.DTOs.UserActivity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IUserActivityService
    {
        Task<IEnumerable<UserActivityResponseDto>> GetAll();
        Task<UserActivityResponseDto?> GetById(string id);
        Task<UserActivityResponseDto> Create(UserActivityCreateDto dto);
        Task<UserActivityResponseDto?> Update(string id, UserActivityCreateDto dto);
        Task<bool> Delete(string id);
    }
}
