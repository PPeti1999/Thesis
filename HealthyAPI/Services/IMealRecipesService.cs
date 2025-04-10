using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IMealRecipesService
    {
        Task<IEnumerable<MealRecipes>> GetAll();
        Task<MealRecipes?> GetById(string id);
        Task<MealRecipes> Create(MealRecipes entity);
        Task<MealRecipes?> Update(string id, MealRecipes entity);
        Task<bool> Delete(string id);
        Task RecalculateMealEntryNutrition(string mealEntryId);
    }
}
