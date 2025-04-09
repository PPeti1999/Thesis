using System;

namespace HealthyAPI.DTOs.Recipe
{
    public class RecipeResponseDto
    {
        public string RecipeID { get; set; }
        public float SumProtein { get; set; }
        public float SumCarb { get; set; }
        public float SumFat { get; set; }
        public float SumCalorie { get; set; }
        public string PhotoID { get; set; }
        public string? PhotoData { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
