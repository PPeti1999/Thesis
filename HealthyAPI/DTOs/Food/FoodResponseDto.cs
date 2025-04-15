using System;

namespace HealthyAPI.DTOs.Food
{
    public class FoodResponseDto
    {
        public string FoodID { get; set; }
        public string Title { get; set; }
        public int Protein { get; set; }
        public int Fat { get; set; }
        public int Carb { get; set; }
        public int Calorie { get; set; }
        public int Gram { get; set; }
        public string? PhotoID { get; set; }
        public string? PhotoData { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
