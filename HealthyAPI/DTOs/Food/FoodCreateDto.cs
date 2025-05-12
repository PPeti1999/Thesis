namespace HealthyAPI.DTOs.Food
{
    public class FoodCreateDto
    {
        public string Title { get; set; }
        public int Protein { get; set; }
        public int Fat { get; set; }
        public int Carb { get; set; }
        public int Calorie { get; set; }
        public int Gram { get; set; }
      /*  public string PhotoData { get; set; } // base64*/
    }
}
