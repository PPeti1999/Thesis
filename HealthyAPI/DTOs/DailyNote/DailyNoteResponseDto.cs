using System;

namespace HealthyAPI.DTOs.DailyNote
{
    public class DailyNoteResponseDto
    {
        public string DailyNoteID { get; set; }
        public string UserID { get; set; }
        public int DailyWeight { get; set; }
        public int DailyTargetCalorie { get; set; }
        public int ActualCalorie { get; set; }
        public float DailyTargetProtein { get; set; }
        public float ActualSumProtein { get; set; }
        public float DailyTargetCarb { get; set; }
        public float ActualSumCarb { get; set; }
        public float DailyTargetFat { get; set; }
        public float ActualSumFat { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
