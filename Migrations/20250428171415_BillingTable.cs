using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyProjectIT15.Migrations
{
    /// <inheritdoc />
    public partial class BillingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "2b477542-3b46-4b53-9364-c42ffa049e64");

            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "2c213701-4a28-4d72-a920-4a4eb0673d17");

            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "f3b1f987-cb7e-45ed-8788-7d1cfba32503");

            migrationBuilder.CreateTable(
                name: "Billings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MeterReadingId = table.Column<int>(type: "int", nullable: true),
                    ReadingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(16,2)", precision: 16, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Billings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Billings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Billings_MeterReadings_MeterReadingId",
                        column: x => x.MeterReadingId,
                        principalTable: "MeterReadings",
                        principalColumn: "Id");
                });

            //migrationBuilder.InsertData(
            //    table: "AspNetRoles",
            //    columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
            //    values: new object[,]
            //    {
            //        { "403fd120-0328-443e-a5a3-f6a2680b7cf2", null, "tenant", "tenant" },
            //        { "9e6a6626-bc78-42db-9279-d5747c1c4246", null, "owner", "owner" },
            //        { "ae5e5d15-50e5-4e7e-869a-856a51d2d05f", null, "admin", "admin" }
            //    });

            migrationBuilder.CreateIndex(
                name: "IX_Billings_MeterReadingId",
                table: "Billings",
                column: "MeterReadingId");

            migrationBuilder.CreateIndex(
                name: "IX_Billings_UserId",
                table: "Billings",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Billings");

            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "403fd120-0328-443e-a5a3-f6a2680b7cf2");

            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "9e6a6626-bc78-42db-9279-d5747c1c4246");

            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "ae5e5d15-50e5-4e7e-869a-856a51d2d05f");

            //migrationBuilder.InsertData(
            //    table: "AspNetRoles",
            //    columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
            //    values: new object[,]
            //    {
            //        { "2b477542-3b46-4b53-9364-c42ffa049e64", null, "admin", "admin" },
            //        { "2c213701-4a28-4d72-a920-4a4eb0673d17", null, "owner", "owner" },
            //        { "f3b1f987-cb7e-45ed-8788-7d1cfba32503", null, "tenant", "tenant" }
            //    });
        }
    }
}
