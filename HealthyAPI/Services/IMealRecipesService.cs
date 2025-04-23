using HealthyAPI.DTOs.MealRecipe;
using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IMealRecipesService
    {
        Task<IEnumerable<MealRecipeResponseDto>> GetAll();
        Task<MealRecipeResponseDto?> GetById(string id);
        Task<MealRecipeResponseDto> Create(MealRecipeCreateDto dto);
        Task<MealRecipeResponseDto?> Update(string id, MealRecipeCreateDto dto);
        Task<bool> Delete(string id);
    }
}
