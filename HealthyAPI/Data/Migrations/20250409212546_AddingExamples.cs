using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HealthyAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingExamples : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "SumCalorie",
                table: "MealEntries",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SumCarb",
                table: "MealEntries",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SumFat",
                table: "MealEntries",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SumProtein",
                table: "MealEntries",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ActualSumCarb",
                table: "DailyNote",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ActualSumFat",
                table: "DailyNote",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ActualSumProtein",
                table: "DailyNote",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "DailyTargetCarb",
                table: "DailyNote",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "DailyTargetFat",
                table: "DailyNote",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "DailyTargetProtein",
                table: "DailyNote",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "TargeProtein",
                table: "AspNetUsers",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "TargetCarb",
                table: "AspNetUsers",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "TargetFat",
                table: "AspNetUsers",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.InsertData(
                table: "ActivityCatalog",
                columns: new[] { "ActivityCatalogID", "Calories", "CreatedAt", "Minute", "Name" },
                values: new object[] { "act1", 300, new DateTime(2025, 4, 9, 23, 25, 46, 161, DateTimeKind.Local).AddTicks(8362), 30, "Futás" });

            migrationBuilder.InsertData(
                table: "Photo",
                columns: new[] { "PhotoID", "PhotoData" },
                values: new object[,]
                {
                    { "photo1", "https://example.com/photo1.png" },
                    { "photo2", "https://example.com/photo2.png" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Age", "BodyFat", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "FirstName", "GoalWeight", "Height", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PhotoID", "SecurityStamp", "TargeProtein", "TargetCalorie", "TargetCarb", "TargetFat", "TwoFactorEnabled", "UserName", "Weight" },
                values: new object[] { "27aca460-6814-41e1-8544-3dfa81a086c3", 0, 25, 20, "b515be15-e82b-4a09-bc47-b4e7da1e0bb8", new DateTime(2025, 4, 9, 21, 25, 46, 163, DateTimeKind.Utc).AddTicks(2114), "pasztoripeti@gmail.com", true, "pásztori", 90, 92, "péter", false, null, "PASZTORIPETI@GMAIL.COM", "PASZTORIPETI@GMAIL.COM", "AQAAAAIAAYagAAAAEAsKi70pulxqK+JDCcrRxAoWjWE40YYxVbY6cKFl92Q42We8obbdm1POcnSrj0SOUg==", null, false, "photo1", "DI4RX3HRROHEWOB3EL52VUN2DPMX2WJT", 180f, 2300, 300f, 70f, false, "pasztoripeti@gmail.com", 92 });

            migrationBuilder.InsertData(
                table: "Food",
                columns: new[] { "FoodID", "Calorie", "Carb", "CreatedAt", "Fat", "Gram", "PhotoID", "Protein", "Title" },
                values: new object[,]
                {
                    { "food1", 165, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 100, "photo1", 31, "Csirkemell" },
                    { "food2", 130, 28, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 100, "photo2", 2, "Rizs" }
                });

            migrationBuilder.InsertData(
                table: "MealTypes",
                columns: new[] { "MealTypeID", "Name", "PhotoID" },
                values: new object[,]
                {
                    { "1", "Reggeli", "photo1" },
                    { "2", "Ebéd", "photo2" }
                });

            migrationBuilder.InsertData(
                table: "Recipe",
                columns: new[] { "RecipeID", "CreatedAt", "PhotoID", "SumCalorie", "SumCarb", "SumFat", "SumProtein" },
                values: new object[] { "rec1", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "photo1", 295f, 28f, 3f, 33f });

            migrationBuilder.InsertData(
                table: "DailyNote",
                columns: new[] { "DailyNoteID", "ActualCalorie", "ActualSumCarb", "ActualSumFat", "ActualSumProtein", "CreatedAt", "DailyTargetCalorie", "DailyTargetCarb", "DailyTargetFat", "DailyTargetProtein", "DailyWeight", "UserID" },
                values: new object[] { "dn1", 0, 0f, 0f, 0f, new DateTime(2025, 4, 9, 23, 25, 46, 159, DateTimeKind.Local).AddTicks(3820), 2300, 300f, 70f, 180f, 92, "27aca460-6814-41e1-8544-3dfa81a086c3" });

            migrationBuilder.InsertData(
                table: "RecipeFoods",
                columns: new[] { "RecipeFoodID", "FoodID", "Quantity", "RecipeID" },
                values: new object[,]
                {
                    { "rf1", "food1", 100f, "rec1" },
                    { "rf2", "food2", 100f, "rec1" }
                });

            migrationBuilder.InsertData(
                table: "MealEntries",
                columns: new[] { "MealEntryID", "DailyNoteID", "MealTypeID", "SumCalorie", "SumCarb", "SumFat", "SumProtein" },
                values: new object[] { "me1", "dn1", "1", 0f, 0f, 0f, 0f });

            migrationBuilder.InsertData(
                table: "UserActivity",
                columns: new[] { "UserActivityID", "ActivityCatalogID", "Calories", "DailyNoteID", "Duration", "PhotoID" },
                values: new object[] { "ua1", null, 300, "dn1", 30, "photo2" });

            migrationBuilder.InsertData(
                table: "MealFoods",
                columns: new[] { "MealFoodID", "FoodID", "MealEntryID", "Quantity" },
                values: new object[,]
                {
                    { "mf1", "food1", "me1", 150 },
                    { "mf2", "food2", "me1", 100 }
                });

            migrationBuilder.InsertData(
                table: "MealRecipes",
                columns: new[] { "MealRecipeID", "MealEntryID", "Quantity", "RecipeID" },
                values: new object[] { "mr1", "me1", 1f, "rec1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ActivityCatalog",
                keyColumn: "ActivityCatalogID",
                keyValue: "act1");

            migrationBuilder.DeleteData(
                table: "MealFoods",
                keyColumn: "MealFoodID",
                keyValue: "mf1");

            migrationBuilder.DeleteData(
                table: "MealFoods",
                keyColumn: "MealFoodID",
                keyValue: "mf2");

            migrationBuilder.DeleteData(
                table: "MealRecipes",
                keyColumn: "MealRecipeID",
                keyValue: "mr1");

            migrationBuilder.DeleteData(
                table: "MealTypes",
                keyColumn: "MealTypeID",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "RecipeFoods",
                keyColumn: "RecipeFoodID",
                keyValue: "rf1");

            migrationBuilder.DeleteData(
                table: "RecipeFoods",
                keyColumn: "RecipeFoodID",
                keyValue: "rf2");

            migrationBuilder.DeleteData(
                table: "UserActivity",
                keyColumn: "UserActivityID",
                keyValue: "ua1");

            migrationBuilder.DeleteData(
                table: "Food",
                keyColumn: "FoodID",
                keyValue: "food1");

            migrationBuilder.DeleteData(
                table: "Food",
                keyColumn: "FoodID",
                keyValue: "food2");

            migrationBuilder.DeleteData(
                table: "MealEntries",
                keyColumn: "MealEntryID",
                keyValue: "me1");

            migrationBuilder.DeleteData(
                table: "Recipe",
                keyColumn: "RecipeID",
                keyValue: "rec1");

            migrationBuilder.DeleteData(
                table: "DailyNote",
                keyColumn: "DailyNoteID",
                keyValue: "dn1");

            migrationBuilder.DeleteData(
                table: "MealTypes",
                keyColumn: "MealTypeID",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "Photo",
                keyColumn: "PhotoID",
                keyValue: "photo2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27aca460-6814-41e1-8544-3dfa81a086c3");

            migrationBuilder.DeleteData(
                table: "Photo",
                keyColumn: "PhotoID",
                keyValue: "photo1");

            migrationBuilder.DropColumn(
                name: "SumCalorie",
                table: "MealEntries");

            migrationBuilder.DropColumn(
                name: "SumCarb",
                table: "MealEntries");

            migrationBuilder.DropColumn(
                name: "SumFat",
                table: "MealEntries");

            migrationBuilder.DropColumn(
                name: "SumProtein",
                table: "MealEntries");

            migrationBuilder.DropColumn(
                name: "ActualSumCarb",
                table: "DailyNote");

            migrationBuilder.DropColumn(
                name: "ActualSumFat",
                table: "DailyNote");

            migrationBuilder.DropColumn(
                name: "ActualSumProtein",
                table: "DailyNote");

            migrationBuilder.DropColumn(
                name: "DailyTargetCarb",
                table: "DailyNote");

            migrationBuilder.DropColumn(
                name: "DailyTargetFat",
                table: "DailyNote");

            migrationBuilder.DropColumn(
                name: "DailyTargetProtein",
                table: "DailyNote");

            migrationBuilder.DropColumn(
                name: "TargeProtein",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TargetCarb",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TargetFat",
                table: "AspNetUsers");
        }
    }
}
