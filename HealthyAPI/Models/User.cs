using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.AspNetCore.Identity;

namespace HealthyAPI.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public int Age { get; set; }
        public int Height { get; set; }
        public int BodyFat { get; set; }
        public int Weight { get; set; }
        public int GoalWeight { get; set; }
        public int TargetCalorie { get; set; }
        public float TargeProtein { get; set; }
        public float TargetCarb { get; set; }
        public float TargetFat { get; set; }
        // FK to Photo (profilfotó)
        public string PhotoID { get; set; }
        [ForeignKey("PhotoID")]
        public virtual Photo Photo { get; set; }
        // A virtual kulcsszó lehetővé teszi az EF Core számára, hogy dinamikus proxy osztályt hozzon létre.
        //Ez a proxy támogatja a lazy loading-ot(lustán betöltést), 
        //így a kapcsolódó Photo entitás csak akkor töltődik be, amikor ténylegesen elérjük a Photo tulajdonságot.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Navigáció: Egy felhasználónak több DailyNote-ja lehet.
      /*  public virtual ICollection<DailyNote> DailyNotes { get; set; }*/
        //Ez egy kényelmi megoldás: a kódolás során nem kell külön lekérdezést írni a kapcsolódó naplók összegyűjtésére, mert az EF Core automatikusan betölti őket (lazy loading vagy eager loading alapján),
        //ha a navigációs tulajdonság szerepel az entitásban.
    }
}
