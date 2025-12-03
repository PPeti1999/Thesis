using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HealthyAPI.Models
{
    public class MealFoods
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string MealFoodID { get; set; }
        public string MealEntryID { get; set; }
        [ForeignKey("MealEntryID")]
        public virtual MealEntries MealEntry { get; set; }

        public string FoodID { get; set; } 
        [ForeignKey("FoodID")]
        public virtual Food Food { get; set; }

        public int Quantity { get; set; }
    }
}
