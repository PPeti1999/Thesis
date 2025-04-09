namespace HealthyAPI.DTOs.Food
{
    public class MealEntryCreateDto
    {
       // MealEntryCreateDto – amit a frontend küld POST/PUT esetén:
        public string DailyNoteID { get; set; }
        public string MealTypeID { get; set; }
    }
}
