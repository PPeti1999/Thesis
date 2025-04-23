using HealthyAPI.DTOs.Recipe;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IRecipeService
    {
        Task<IEnumerable<RecipeResponseDto>> GetAll();
        Task<RecipeResponseDto?> GetById(string id);
        Task<RecipeResponseDto> Create(RecipeCreateDto dto);
        Task<RecipeResponseDto?> Update(string id, RecipeUpdateDto dto);
        Task<bool> Delete(string id);
    }
}
