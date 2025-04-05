using HealthyAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthyAPI.Data
{
    public class Context : IdentityDbContext<User>

    {
        public Context(DbContextOptions<Context> options)
        : base(options)
        {
            //  InitializeData();
        }
        public DbSet<Food> Food { get; set; } = default!;
        public DbSet<Recipe> Recipe { get; set; } = default!;
        public DbSet<Photo> Photo { get; set; } = default!;
        public DbSet<UserActivity> UserActivity { get; set; } = default!;
        public DbSet<MealTypes> MealTypes { get; set; } = default!;
        public DbSet<DailyNote> DailyNote { get; set; } = default!;
        public DbSet<ActivityCatalog> ActivityCatalog { get; set; } = default!;
        public DbSet<MealEntries> MealEntries { get; set; } = default!;
        public DbSet<MealFoods> MealFoods { get; set; } = default!;
        public DbSet<MealTypes> User { get; set; } = default!;
        public DbSet<MealRecipes> MealRecipes { get; set; } = default!;
        public DbSet<RecipeFoods> RecipeFoods { get; set; } = default!;
       


    }
}
