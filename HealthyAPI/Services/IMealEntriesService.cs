using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IMealEntriesService
    {
        Task<IEnumerable<MealEntries>> GetAllMealEntries();
        Task<MealEntries?> GetById(string id);
        Task<MealEntries> Create(MealEntries mealEntry);
        Task<MealEntries?> Update(string id, MealEntries updated);
        Task<bool> Delete(string id);
    }
}
