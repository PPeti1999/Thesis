namespace HealthyAPI.DTOs.UserActivity
{
    public class UserActivityResponseDto
    {
        public string UserActivityID { get; set; }
        public string DailyNoteID { get; set; }
        public string ActivityCatalogID { get; set; }
        public string ActivityName { get; set; }
        public int Duration { get; set; }
        public int Calories { get; set; }
        public string? PhotoID { get; set; }
        public string? PhotoData { get; set; }
    }
}
