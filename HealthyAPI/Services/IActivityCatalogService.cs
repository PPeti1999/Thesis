using HealthyAPI.DTOs.ActivityCatalog;
using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IActivityCatalogService
    {
        Task<IEnumerable<ActivityCatalog>> GetAll();
        Task<ActivityCatalog> GetById(string id);
        Task<ActivityCatalog> Create(ActivityCatalog dto);
        Task<ActivityCatalog> Update(string id, ActivityCatalog dto);
        Task<bool> Delete(string id);
    }
}
