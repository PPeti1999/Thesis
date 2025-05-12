using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace HealthyAPI.Models
{
    public class Food
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]// szöveges id generálás
        public string FoodID { get; set; }
        public string Title { get; set; }
        public int Protein { get; set; }
        public int Fat { get; set; }
        public int Carb { get; set; }
        public int Calorie { get; set; }
        public int Gram { get; set; }

      /*  public string PhotoID { get; set; }
        [ForeignKey("PhotoID")]
        public virtual Photo Photo { get; set; }
      */
        public DateTime CreatedAt { get; set; }

        //     public virtual ICollection<MealFoods> MealFoods { get; set; }
        //    public virtual ICollection<RecipeFoods> RecipeFoods { get; set; }
    }
}
