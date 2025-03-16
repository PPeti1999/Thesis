using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HealthyAPI.Models
{
    public class UserActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]// szöveges id generálás
        public string UserActivityID { get; set; }

        public string DailyNoteID { get; set; }
        [ForeignKey("DailyNoteID")]
        public virtual DailyNote DailyNote { get; set; }

        // FK az ActivityCatalog-ból (az aktivitás kiválasztása)
        public string ActivityCatalogID { get; set; }
        [ForeignKey("ActivityCatalogID")]
        public virtual ActivityCatalog ActivityCatalog { get; set; }

        // Időtartam percben
        public int Duration { get; set; }
        // Kiszámolt elégetett kalória
        public int Calories { get; set; }

        // Fotó, ha a felhasználó az aktivitásról képet is csatol
        public string PhotoID { get; set; }
        [ForeignKey("PhotoID")]
        public virtual Photo Photo { get; set; }
    }
}
