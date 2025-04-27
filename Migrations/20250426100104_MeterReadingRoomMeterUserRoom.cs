using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyProjectIT15.Migrations
{
    /// <inheritdoc />
    public partial class MeterReadingRoomMeterUserRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeterReadings_Meters_MeterId",
                table: "MeterReadings");

            migrationBuilder.DropForeignKey(
                name: "FK_MeterReadings_Rooms_RoomId",
                table: "MeterReadings");

            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "ac6416e7-3cd6-4a6f-9dba-4c31dfb493de");

            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "c80aa919-9854-45a9-a150-fb910be34755");

            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "eb09b41b-1013-4be1-a146-54b0ccde2adc");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "MeterReadings",
                newName: "UserRoomId");

            migrationBuilder.RenameColumn(
                name: "MeterId",
                table: "MeterReadings",
                newName: "RoomMeterId");

            migrationBuilder.RenameIndex(
                name: "IX_MeterReadings_RoomId",
                table: "MeterReadings",
                newName: "IX_MeterReadings_UserRoomId");

            migrationBuilder.RenameIndex(
                name: "IX_MeterReadings_MeterId",
                table: "MeterReadings",
                newName: "IX_MeterReadings_RoomMeterId");

            migrationBuilder.CreateTable(
                name: "RoomMeters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: true),
                    MeterId = table.Column<int>(type: "int", nullable: true),
                    installedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomMeters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomMeters_Meters_MeterId",
                        column: x => x.MeterId,
                        principalTable: "Meters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoomMeters_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id");
                });

            //migrationBuilder.InsertData(
            //    table: "AspNetRoles",
            //    columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
            //    values: new object[,]
            //    {
            //        { "9a1816ae-39ee-4898-9ab8-6287b3195e06", null, "admin", "admin" },
            //        { "bd886912-c53a-4280-8709-8785e023f6c5", null, "tenant", "tenant" },
            //        { "efb97baa-d40f-4f4c-91b8-000323360d20", null, "owner", "owner" }
            //    });

            migrationBuilder.CreateIndex(
                name: "IX_RoomMeters_MeterId",
                table: "RoomMeters",
                column: "MeterId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomMeters_RoomId",
                table: "RoomMeters",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeterReadings_RoomMeters_RoomMeterId",
                table: "MeterReadings",
                column: "RoomMeterId",
                principalTable: "RoomMeters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MeterReadings_UserRooms_UserRoomId",
                table: "MeterReadings",
                column: "UserRoomId",
                principalTable: "UserRooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeterReadings_RoomMeters_RoomMeterId",
                table: "MeterReadings");

            migrationBuilder.DropForeignKey(
                name: "FK_MeterReadings_UserRooms_UserRoomId",
                table: "MeterReadings");

            migrationBuilder.DropTable(
                name: "RoomMeters");

            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "9a1816ae-39ee-4898-9ab8-6287b3195e06");

            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "bd886912-c53a-4280-8709-8785e023f6c5");

            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "efb97baa-d40f-4f4c-91b8-000323360d20");

            migrationBuilder.RenameColumn(
                name: "UserRoomId",
                table: "MeterReadings",
                newName: "RoomId");

            migrationBuilder.RenameColumn(
                name: "RoomMeterId",
                table: "MeterReadings",
                newName: "MeterId");

            migrationBuilder.RenameIndex(
                name: "IX_MeterReadings_UserRoomId",
                table: "MeterReadings",
                newName: "IX_MeterReadings_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_MeterReadings_RoomMeterId",
                table: "MeterReadings",
                newName: "IX_MeterReadings_MeterId");

            //migrationBuilder.InsertData(
            //    table: "AspNetRoles",
            //    columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
            //    values: new object[,]
            //    {
            //        { "ac6416e7-3cd6-4a6f-9dba-4c31dfb493de", null, "tenant", "tenant" },
            //        { "c80aa919-9854-45a9-a150-fb910be34755", null, "owner", "owner" },
            //        { "eb09b41b-1013-4be1-a146-54b0ccde2adc", null, "admin", "admin" }
            //    });

            migrationBuilder.AddForeignKey(
                name: "FK_MeterReadings_Meters_MeterId",
                table: "MeterReadings",
                column: "MeterId",
                principalTable: "Meters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MeterReadings_Rooms_RoomId",
                table: "MeterReadings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");
        }
    }
}
