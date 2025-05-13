namespace HealthyAPI.DTOs.Profile
{
    public class UserProfileResponseDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int Height { get; set; }
        public int BodyFat { get; set; }
        public int Weight { get; set; }
        public int GoalWeight { get; set; }
        public int TargetCalorie { get; set; }
        public float TargetProtein { get; set; }
        public float TargetCarb { get; set; }
        public float TargetFat { get; set; }
        public string PhotoID { get; set; }
        public string? PhotoData { get; set; }
        public bool IsFemale { get; set; } // true = nő, false = férfi
        public int GoalType { get; set; } // 0 = megtartás, 1 = tömegnövelés, 2 = fogyás
        public float ActivityMultiplier { get; set; } // ÚJ mező


    }
}
