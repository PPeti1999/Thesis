using HealthyAPI.DTOs.MealType;
using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IMealTypeService
    {
        Task<IEnumerable<MealTypeResponseDto>> GetAll();
        Task<MealTypeResponseDto> GetById(string id);
        Task<MealTypeResponseDto> Create(MealTypeCreateDto entity);
        Task<MealTypeResponseDto> Update(string id, MealTypeCreateDto updated);
        Task<bool> Delete(string id);
    }
}
