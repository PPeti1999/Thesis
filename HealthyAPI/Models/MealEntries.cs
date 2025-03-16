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

        // Az étkezés típusát az FK(MealTypeID) alapján választjuk ki.
        public string MealTypeID { get; set; }
        [ForeignKey("MealTypeID")]
        public virtual MealTypes MealType { get; set; }

        // Navigáció: Egy étkezéshez több étel (MealFood) és recept (MealRecipe) tartozhat.
        public virtual ICollection<MealFoods> MealFoods { get; set; }
        public virtual ICollection<MealRecipes> MealRecipes { get; set; }
    }
}
