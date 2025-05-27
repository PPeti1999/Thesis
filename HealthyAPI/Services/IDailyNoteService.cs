using HealthyAPI.DTOs.CalendarSummary;
using HealthyAPI.DTOs.DailyNote;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IDailyNoteService
    {
        Task<DailyNoteResponseDto?> GetTodayNote(string userId);
        Task<DailyNoteResponseDto> CreateDailyNote(string userId);
        Task UpdateMealNutritionAsync(string dailyNoteId);
      
        Task<DailyNoteResponseDto?> UpdateWeight(string dailyNoteId, int weight);
        Task<DailyNoteResponseDto?> GetPreviousNote(string userId, DateTime currentDate);
        Task<DailyNoteResponseDto?> GetNextNote(string userId, DateTime currentDate);

        Task<DailyNoteResponseDto?> GetNoteByDate(string userId, DateTime date); // 👈 új a naptárhoz
        Task<List<CalendarSummaryDto>> GetMonthlySummaryAsync(string userId, int year, int month);

    }
}
