using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthyAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingggggUserToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "ActivityMultiplier",
                table: "AspNetUsers",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<bool>(
                name: "IsFemale",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27aca460-6814-41e1-8544-3dfa81a086c3",
                columns: new[] { "ActivityMultiplier", "IsFemale" },
                values: new object[] { 1f, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityMultiplier",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsFemale",
                table: "AspNetUsers");
        }
    }
}
