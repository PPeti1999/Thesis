namespace HealthyAPI.DTOs.RecipeFood
{
    public class RecipeFoodResponseDto
    {
        public string RecipeFoodID { get; set; }
        public string RecipeID { get; set; }
        public string FoodID { get; set; }
        public string? FoodName { get; set; }
        public float Quantity { get; set; }
    }
}
