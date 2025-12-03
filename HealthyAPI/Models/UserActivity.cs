using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HealthyAPI.Models
{
    public class UserActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string UserActivityID { get; set; }

        public string DailyNoteID { get; set; }
        [ForeignKey("DailyNoteID")]
        public virtual DailyNote DailyNote { get; set; }
        public string ActivityCatalogID { get; set; }
        [ForeignKey("ActivityCatalogID")]
        public virtual ActivityCatalog ActivityCatalog { get; set; }
        public int Duration { get; set; }

        public int Calories { get; set; }
    }
}
