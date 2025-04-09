using System;

namespace HealthyAPI.DTOs.MealEntries
{
    public class MealEntryResponseDto
    {
        //MealEntryResponseDto – amit visszaküldünk a frontendnek:
        public string MealEntryID { get; set; }
        public string DailyNoteID { get; set; }
        public DateTime? DailyNoteCreatedAt { get; set; }

        public string MealTypeID { get; set; }
        public string MealTypeName { get; set; }
    }
}
