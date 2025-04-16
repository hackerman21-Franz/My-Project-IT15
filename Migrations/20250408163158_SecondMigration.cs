using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyProjectIT15.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8e44d1ea-7c2c-43f9-85c0-43ea9c5af9d8", null, "admin", "admin" },
                    { "908cda4d-0527-4fae-9a8d-503005f3ba09", null, "owner", "owner" },
                    { "c1c93f6b-b4a2-43f2-b234-93f24f8a6c36", null, "tenant", "tenant" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8e44d1ea-7c2c-43f9-85c0-43ea9c5af9d8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "908cda4d-0527-4fae-9a8d-503005f3ba09");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c1c93f6b-b4a2-43f2-b234-93f24f8a6c36");
        }
    }
}
