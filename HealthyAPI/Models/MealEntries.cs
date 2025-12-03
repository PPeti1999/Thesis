using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HealthyAPI.Models
{
    public class MealEntries
    {
        [Key]
        public string MealEntryID { get; set; }

        public string DailyNoteID { get; set; }
        [ForeignKey("DailyNoteID")]
        public virtual DailyNote DailyNote { get; set; }

        public string MealTypeID { get; set; }
        [ForeignKey("MealTypeID")]
        public virtual MealTypes MealType { get; set; }
        public float SumProtein { get; set; }
        public float SumCarb { get; set; }
        public float SumFat { get; set; }
        public float SumCalorie { get; set; }
         public virtual ICollection<MealFoods> MealFoods { get; set; }
         public virtual ICollection<MealRecipes> MealRecipes { get; set; }
    }
}
