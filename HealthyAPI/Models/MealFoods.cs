using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HealthyAPI.Models
{
    public class MealFoods
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]// szöveges id generálás
        public string MealFoodID { get; set; }
        public string MealEntryID { get; set; }//melyik étkezéshez
        [ForeignKey("MealEntryID")]
        public virtual MealEntries MealEntry { get; set; }

        public string FoodID { get; set; } //melyik alapanyagból
        [ForeignKey("FoodID")]
        public virtual Food Food { get; set; }

        // Mennyiség grammban
        public int Quantity { get; set; }
    }
}
