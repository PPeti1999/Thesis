using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IMealTypeService
    {
        Task<IEnumerable<MealTypes>> GetAll();
        Task<MealTypes?> GetById(string id);
        Task<MealTypes> Create(MealTypes entity);
        Task<MealTypes?> Update(string id, MealTypes updated);
        Task<bool> Delete(string id);
    }
}
