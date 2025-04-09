using System.Collections.Generic;

namespace HealthyAPI.DTOs.Recipe
{
    public class RecipeCreateDto
    {
        public string PhotoID { get; set; }
        public List<RecipeFoodItemDto> Ingredients { get; set; }
    }
}
