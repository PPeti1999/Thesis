using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace HealthyAPI.Models
{
    public class DailyNote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]// szöveges id generálás
        public string DailyNoteID { get; set; }
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        public int DailyTargetCalorie { get; set; }
        public int ActualCalorie { get; set; }
        public int DailyWeight { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigáció: A DailyNote több étkezést (MealEntry) és aktivitást tartalmazhat.
            public virtual ICollection<MealEntries> MealEntries { get; set; }
            public virtual ICollection<UserActivity> UserActivities { get; set; }
    }
}
