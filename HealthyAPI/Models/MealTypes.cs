using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HealthyAPI.Models
{
    public class MealTypes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]// szöveges id generálás
        public string MealTypeID { get; set; }
        public string Name { get; set; } // Pl.: "Reggeli", "Ebéd", "Vacsora", "Nasi"

        // Az étkezéstípushoz tartozó ikon/fotó
        public string PhotoID { get; set; }
        [ForeignKey("PhotoID")]
        public virtual Photo Photo { get; set; }
    }
}
