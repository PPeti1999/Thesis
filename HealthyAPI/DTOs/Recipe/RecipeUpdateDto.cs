using System.Collections.Generic;

namespace HealthyAPI.DTOs.Recipe
{
    public class RecipeUpdateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string PhotoID { get; set; }
        public List<RecipeFoodItemDto> Ingredients { get; set; }
    }
}
