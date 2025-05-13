using HealthyAPI.DTOs.Food;
using HealthyAPI.DTOs.MealEntries;
using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IMealEntriesService
    {
        Task<IEnumerable<MealEntryResponseDto>> GetAll();
        Task<MealEntryResponseDto?> GetById(string id);
        Task<MealEntryResponseDto> Create(MealEntryCreateDto dto);
        Task<IEnumerable<MealEntryResponseDto>> GetByDailyNoteId(string dailyNoteId);

        Task<MealEntryResponseDto?> Update(string id, MealEntryCreateDto dto);
        Task<bool> Delete(string id);
    }
}
