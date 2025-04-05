using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Repositories
{
    public interface IFoodRepository
    {
        Task<IEnumerable<Food>> ListFoods();
        Task<Food> GetFood(string id);
        Task<Food> AddFood(Food food);
        Task<Food> UpdateFood(Food food);
        Task<bool> DeleteFood(string id);
    }
}
