using HealthyAPI.DTOs.ActivityCatalog;
using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IActivityCatalogService
    {
        Task<IEnumerable<ActivityCatalogResponseDto>> GetAll();
        Task<ActivityCatalogResponseDto> GetById(string id);
    /*
        Task<ActivityCatalog> Create(ActivityCatalog dto);
        Task<ActivityCatalog> Update(string id, ActivityCatalog dto);*/
    // Itt változott a paraméter ActivityCatalog-ról ActivityCatalogCreateDto-ra:
    Task<ActivityCatalogResponseDto> Create(ActivityCatalogCreateDto dto);
    Task<ActivityCatalogResponseDto> Update(string id, ActivityCatalogCreateDto dto);
    Task<bool> Delete(string id);
    }
}
