using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthyAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingExamples2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ActivityCatalog",
                keyColumn: "ActivityCatalogID",
                keyValue: "act1",
                column: "CreatedAt",
                value: new DateTime(2024, 4, 9, 8, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27aca460-6814-41e1-8544-3dfa81a086c3",
                column: "CreatedAt",
                value: new DateTime(2024, 4, 9, 8, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "DailyNote",
                keyColumn: "DailyNoteID",
                keyValue: "dn1",
                column: "CreatedAt",
                value: new DateTime(2024, 4, 9, 8, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ActivityCatalog",
                keyColumn: "ActivityCatalogID",
                keyValue: "act1",
                column: "CreatedAt",
                value: new DateTime(2025, 4, 9, 23, 25, 46, 161, DateTimeKind.Local).AddTicks(8362));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27aca460-6814-41e1-8544-3dfa81a086c3",
                column: "CreatedAt",
                value: new DateTime(2025, 4, 9, 21, 25, 46, 163, DateTimeKind.Utc).AddTicks(2114));

            migrationBuilder.UpdateData(
                table: "DailyNote",
                keyColumn: "DailyNoteID",
                keyValue: "dn1",
                column: "CreatedAt",
                value: new DateTime(2025, 4, 9, 23, 25, 46, 159, DateTimeKind.Local).AddTicks(3820));
        }
    }
}
