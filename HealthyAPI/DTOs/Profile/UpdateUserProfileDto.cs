namespace HealthyAPI.DTOs.Profile
{
    public class UpdateUserProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int Height { get; set; }
        public int BodyFat { get; set; }
        public int Weight { get; set; }
        public int GoalWeight { get; set; }
        public float ActivityMultiplier { get; set; } // pl. 1.2 vagy 1.55
        public int GoalType { get; set; } // 0 = megtartás, 1 = tömegnövelés, 2 = fogyás
        public bool IsFemale { get; set; } // true = nő, false = férfi
    }

}
