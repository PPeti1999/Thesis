using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IMealFoodsService
    {
        Task<IEnumerable<MealFoods>> GetAllMealFoods();
        Task<MealFoods?> GetByIdMealFoods(string id);
        Task<IEnumerable<MealFoods>> GetMealFoodsByMealEntryId(string mealEntryId);
        Task<MealFoods> CreateMealFoods(MealFoods mealFood);
        Task<MealFoods?> UpdateMealFoods(string id, MealFoods updated);
        Task<bool> DeleteMealFoods(string id);
        Task RecalculateMealEntryNutrition(string mealEntryId);
    }
}
