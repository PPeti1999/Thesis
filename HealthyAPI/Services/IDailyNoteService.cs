using HealthyAPI.DTOs.DailyNote;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IDailyNoteService
    {
        Task<DailyNoteResponseDto?> GetTodayNote(string userId);
        Task<DailyNoteResponseDto> CreateDailyNote(string userId);
        Task UpdateMealNutritionAsync(string dailyNoteId);
      
        Task<DailyNoteResponseDto?> UpdateWeight(string dailyNoteId, int weight);
    }
}
