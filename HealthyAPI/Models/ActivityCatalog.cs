using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace HealthyAPI.Models
{
    public class ActivityCatalog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]// szöveges id generálás
        public string ActivityCatalogID { get; set; }
        public string Name { get; set; }
        public int Minute { get; set; }
        public int Calories { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<UserActivity> UserActivities { get; set; }

    }
}
