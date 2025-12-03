using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HealthyAPI.Models
{
    public class MealTypes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string MealTypeID { get; set; }
        public string Name { get; set; } 
    }
}
