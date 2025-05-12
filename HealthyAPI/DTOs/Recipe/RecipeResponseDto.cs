using System;
using System.Collections.Generic;

namespace HealthyAPI.DTOs.Recipe
{
    public class RecipeResponseDto
    {
        public string RecipeID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public float SumProtein { get; set; }
        public float SumCarb { get; set; }
        public float SumFat { get; set; }
        public float SumCalorie { get; set; }
      /*  public string PhotoID { get; set; }
       public string? PhotoData { get; set; }*/
        public DateTime CreatedAt { get; set; }
        public List<RecipeIngredientDetailDto> Ingredients { get; set; }
    }
}
