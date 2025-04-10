namespace HealthyAPI.DTOs.MealRecipe
{
    public class MealRecipeCreateDto
    {
        public string MealEntryID { get; set; }
        public string RecipeID { get; set; }
        public float Quantity { get; set; }
    }
}
