using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthyAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddinggggggUserToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GoalType",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27aca460-6814-41e1-8544-3dfa81a086c3",
                column: "GoalType",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoalType",
                table: "AspNetUsers");
        }
    }
}
