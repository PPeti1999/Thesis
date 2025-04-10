namespace HealthyAPI.DTOs.UserActivity
{
    public class UserActivityCreateDto
    {
        public string DailyNoteID { get; set; }
        public string ActivityCatalogID { get; set; }
        public int Duration { get; set; }
        public string PhotoID { get; set; }
    }
}
