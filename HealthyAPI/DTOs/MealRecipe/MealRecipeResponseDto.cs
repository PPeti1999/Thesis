namespace HealthyAPI.DTOs.MealRecipe
{
    public class MealRecipeResponseDto
    {
        public string MealRecipeID { get; set; }
        public string MealEntryID { get; set; }
        public string RecipeID { get; set; }
        public string? RecipeTitle { get; set; }
        public float Quantity { get; set; }
    }
}
