using HealthyAPI.DTOs.MealFoods;
using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IMealFoodsService
    {
        Task<IEnumerable<MealFoodResponseDto>> GetAllMealFoods();
        Task<MealFoodResponseDto?> GetByIdMealFoods(string id);
        Task<IEnumerable<MealFoodResponseDto>> GetMealFoodsByMealEntryId(string mealEntryId);
        Task<MealFoodResponseDto> CreateMealFoods(MealFoodCreateDto dto);
        Task<MealFoodResponseDto?> UpdateMealFoods(string id, MealFoodCreateDto dto);
        Task<bool> DeleteMealFoods(string id);
        Task RecalculateMealEntryNutrition(string mealEntryId);
        Task<IEnumerable<MealFoodResponseDto>> GetByMealEntryIdAsync(string mealEntryId);
    }

}
