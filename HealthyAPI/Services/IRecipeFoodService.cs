using HealthyAPI.DTOs.RecipeFood;
using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IRecipeFoodService
    {
        Task<IEnumerable<RecipeFoodResponseDto>> GetAll();
        Task<RecipeFoodResponseDto> GetById(string id);
        Task<RecipeFoodResponseDto> Create(RecipeFoodCreateDto entity);
        Task<RecipeFoodResponseDto> Update(string id, RecipeFoodCreateDto updated);
        Task<bool> Delete(string id);
    }
}
