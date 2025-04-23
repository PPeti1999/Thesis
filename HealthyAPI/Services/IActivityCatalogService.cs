using HealthyAPI.DTOs.ActivityCatalog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IActivityCatalogService
    {
        Task<IEnumerable<ActivityCatalogResponseDto>> GetAll();
        Task<ActivityCatalogResponseDto?> GetById(string id);
        Task<ActivityCatalogResponseDto> Create(ActivityCatalogCreateDto dto);
        Task<ActivityCatalogResponseDto?> Update(string id, ActivityCatalogCreateDto dto);
        Task<bool> Delete(string id);
    }
}
