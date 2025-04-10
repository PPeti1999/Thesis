using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthyAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTitleAndDescriptionToRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Recipe",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Recipe",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Recipe",
                keyColumn: "RecipeID",
                keyValue: "rec1",
                columns: new[] { "Description", "Title" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Recipe");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Recipe");
        }
    }
}
