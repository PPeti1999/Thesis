using HealthyAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace HealthyAPI.Data
{
    public class Context : IdentityDbContext<User>

    {
        public Context(DbContextOptions<Context> options)
        : base(options)
        {
            //  InitializeData();
        }
        /* SQl
         * INSERT INTO DailyNote (DailyNoteID, UserID, DailyTargetCalorie, ActualCalorie, DailyWeight, CreatedAt)
            VALUES (
              '1',         -- DailyNoteID
              '27aca460-6814-41e1-8544-3dfa81a086c3',      -- UserID / ProfileID (csak ha ez létezik!)
              2200,                  -- DailyTargetCalorie
              1900,                  -- ActualCalorie
              75,                    -- DailyWeight (kg)
              GETDATE()              -- CreatedAt
            );
        INSERT INTO MealTypes (MealTypeID, Name, PhotoID)
        VALUES ('1', 'Reggeli', NULL);
        INSERT INTO MealEntries (MealEntryID, MealTypeID, DailyNoteID)
        VALUES ('1', '1', '1');

         */
        public DbSet<Food> Food { get; set; } = default!;
        public DbSet<Recipe> Recipe { get; set; } = default!;
        public DbSet<Photo> Photo { get; set; } = default!;
        public DbSet<UserActivity> UserActivity { get; set; } = default!;
        public DbSet<MealTypes> MealTypes { get; set; } = default!;
        public DbSet<DailyNote> DailyNote { get; set; } = default!;
        public DbSet<ActivityCatalog> ActivityCatalog { get; set; } = default!;
        public DbSet<MealEntries> MealEntries { get; set; } = default!;
        public DbSet<MealFoods> MealFoods { get; set; } = default!;
        public DbSet<MealRecipes> MealRecipes { get; set; } = default!;
        public DbSet<RecipeFoods> RecipeFoods { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Photo
            var photo1 = new Photo { PhotoID = "photo1", PhotoData = "https://example.com/photo1.png" };
            var photo2 = new Photo { PhotoID = "photo2", PhotoData = "https://example.com/photo2.png" };
            modelBuilder.Entity<Photo>().HasData(photo1, photo2);

            // MealTypes
            modelBuilder.Entity<MealTypes>().HasData(
                new MealTypes { MealTypeID = "1", Name = "Reggeli", PhotoID = "photo1" },
                new MealTypes { MealTypeID = "2", Name = "Ebéd", PhotoID = "photo2" }
            );

            // Food
            modelBuilder.Entity<Food>().HasData(
               /*  new Food { FoodID = "food1", Title = "Csirkemell", Protein = 31, Fat = 3, Carb = 0, Calorie = 165, Gram = 100, PhotoID = "photo1" },
                 new Food { FoodID = "food2", Title = "Rizs", Protein = 2, Fat = 0, Carb = 28, Calorie = 130, Gram = 100, PhotoID = "photo2" }*/
               new Food { FoodID = "food1", Title = "Csirkemell", Protein = 31, Fat = 3, Carb = 0, Calorie = 165, Gram = 100},
                new Food { FoodID = "food2", Title = "Rizs", Protein = 2, Fat = 0, Carb = 28, Calorie = 130, Gram = 100 }
            );

            // Recipe
            modelBuilder.Entity<Recipe>().HasData(
                new Recipe { RecipeID = "rec1", SumProtein = 33, SumCarb = 28, SumFat = 3, SumCalorie = 295 }
           
           // new Recipe { RecipeID = "rec1", SumProtein = 33, SumCarb = 28, SumFat = 3, SumCalorie = 295, PhotoID = "photo1" }
            );

            // RecipeFoods
            modelBuilder.Entity<RecipeFoods>().HasData(
                new RecipeFoods { RecipeFoodID = "rf1", RecipeID = "rec1", FoodID = "food1", Quantity = 100 },
                new RecipeFoods { RecipeFoodID = "rf2", RecipeID = "rec1", FoodID = "food2", Quantity = 100 }
            );

            // DailyNote
            modelBuilder.Entity<DailyNote>().HasData(
                new DailyNote
                {
                    DailyNoteID = "dn1",
                    UserID = "27aca460-6814-41e1-8544-3dfa81a086c3",
                    DailyWeight = 92,

                    DailyTargetCalorie = 2300,
                    ActualCalorie = 0,

                    DailyTargetProtein=180,
                    ActualSumProtein=0,
                    DailyTargetCarb=300,
                    ActualSumCarb=0,
                    DailyTargetFat=70,
                    ActualSumFat=0,

                    CreatedAt = new DateTime(2024, 04, 09, 8, 0, 0)
                }
            );

            // MealEntries
            modelBuilder.Entity<MealEntries>().HasData(
                new MealEntries { MealEntryID = "me1", DailyNoteID = "dn1", MealTypeID = "1" }
            );

            // MealFoods
            modelBuilder.Entity<MealFoods>().HasData(
                new MealFoods { MealFoodID = "mf1", MealEntryID = "me1", FoodID = "food1", Quantity = 150 },
                new MealFoods { MealFoodID = "mf2", MealEntryID = "me1", FoodID = "food2", Quantity = 100 }
            );

            // MealRecipes
            modelBuilder.Entity<MealRecipes>().HasData(
                new MealRecipes { MealRecipeID = "mr1", MealEntryID = "me1", RecipeID = "rec1", Quantity = 1 }
            );

            // ActivityCatalog
            modelBuilder.Entity<ActivityCatalog>().HasData(
                new ActivityCatalog { ActivityCatalogID = "act1", Name = "Futás", Minute = 30, Calories = 300, CreatedAt = new DateTime(2024, 04, 09, 8, 0, 0) }
            );

            // UserActivity
            modelBuilder.Entity<UserActivity>().HasData(
                new UserActivity { UserActivityID = "ua1", DailyNoteID = "dn1", Duration = 30, Calories = 300, PhotoID = "photo2" }
            );

           modelBuilder.Entity<User>().HasData(
           new User
           {
               Id = "27aca460-6814-41e1-8544-3dfa81a086c3",
               FirstName = "pásztori",
               LastName = "péter",
               Age= 25,
               Height=92,
               BodyFat=20,
               Weight=92,
               GoalWeight=90,
               TargetCalorie=2300,
               TargeProtein = 180,
               TargetCarb = 300,
               TargetFat  =70,
               ActivityMultiplier=1,
               IsFemale = false,
               PhotoID = "photo1",
               CreatedAt = new DateTime(2024, 04, 09, 8, 0, 0),
               UserName = "pasztoripeti@gmail.com",
               NormalizedUserName = "PASZTORIPETI@GMAIL.COM",
               Email = "pasztoripeti@gmail.com",
               NormalizedEmail = "PASZTORIPETI@GMAIL.COM",
               EmailConfirmed = true,
               PasswordHash = "AQAAAAIAAYagAAAAEAsKi70pulxqK+JDCcrRxAoWjWE40YYxVbY6cKFl92Q42We8obbdm1POcnSrj0SOUg==", // Jelszó: 123456
               SecurityStamp = "DI4RX3HRROHEWOB3EL52VUN2DPMX2WJT",
               ConcurrencyStamp = "b515be15-e82b-4a09-bc47-b4e7da1e0bb8",


           }
       );
        }

    }
}
