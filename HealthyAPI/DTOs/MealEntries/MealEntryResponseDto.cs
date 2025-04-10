using System;

namespace HealthyAPI.DTOs.MealEntries
{
    public class MealEntryResponseDto
    {
        public string MealEntryID { get; set; }
        public string DailyNoteID { get; set; }
        public string MealTypeID { get; set; }
        public string MealTypeName { get; set; }
        public float SumProtein { get; set; }
        public float SumCarb { get; set; }
        public float SumFat { get; set; }
        public float SumCalorie { get; set; }
    }
}
