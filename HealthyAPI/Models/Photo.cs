using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HealthyAPI.Models
{
    public class Photo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]// szöveges id generálás
        public string PhotoID { get; set; }
        // Tárolhat URL-t, base64 kódolt sztringet, stb.
        public string PhotoData { get; set; }

        // Navigációs tulajdonságok, hogy mely entitások használják ezt a fotót
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Food> Foods { get; set; }
        public virtual ICollection<Recipe> Recipes { get; set; }
        public virtual ICollection<UserActivity> UserActivities { get; set; }
        public virtual ICollection<MealTypes> MealTypes { get; set; }
    }
}
