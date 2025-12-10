using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.AspNetCore.Identity;

namespace HealthyAPI.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public int Age { get; set; }
        public int Height { get; set; }
        public int BodyFat { get; set; }
        public int Weight { get; set; }
        public int GoalWeight { get; set; }
        public int TargetCalorie { get; set; }
        public float TargeProtein { get; set; }
        public float TargetCarb { get; set; }
        public float TargetFat { get; set; }
        public float ActivityMultiplier { get; set; } // pl. 1.2, 1.55
        public int GoalType { get; set; } // 0 = megtartás,1 = tömegnövelés,2 = fogyás
        public bool IsFemale { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public class ServiceResult
    {
      public bool Success { get; set; }
      public string Title { get; set; }
      public string Message { get; set; }
    }
  }
}
