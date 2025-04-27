using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyProjectIT15.Migrations
{
    /// <inheritdoc />
    public partial class FixRoomMeter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomMeters_Meters_MeterId",
                table: "RoomMeters");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomMeters_Rooms_RoomId",
                table: "RoomMeters");

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

            migrationBuilder.AlterColumn<int>(
                name: "RoomId",
                table: "RoomMeters",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MeterId",
                table: "RoomMeters",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "RoomMeters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            //migrationBuilder.InsertData(
            //    table: "AspNetRoles",
            //    columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
            //    values: new object[,]
            //    {
            //        { "2b477542-3b46-4b53-9364-c42ffa049e64", null, "admin", "admin" },
            //        { "2c213701-4a28-4d72-a920-4a4eb0673d17", null, "owner", "owner" },
            //        { "f3b1f987-cb7e-45ed-8788-7d1cfba32503", null, "tenant", "tenant" }
            //    });

            migrationBuilder.AddForeignKey(
                name: "FK_RoomMeters_Meters_MeterId",
                table: "RoomMeters",
                column: "MeterId",
                principalTable: "Meters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomMeters_Rooms_RoomId",
                table: "RoomMeters",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomMeters_Meters_MeterId",
                table: "RoomMeters");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomMeters_Rooms_RoomId",
                table: "RoomMeters");

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

            migrationBuilder.DropColumn(
                name: "Status",
                table: "RoomMeters");

            migrationBuilder.AlterColumn<int>(
                name: "RoomId",
                table: "RoomMeters",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MeterId",
                table: "RoomMeters",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            //migrationBuilder.InsertData(
            //    table: "AspNetRoles",
            //    columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
            //    values: new object[,]
            //    {
            //        { "9a1816ae-39ee-4898-9ab8-6287b3195e06", null, "admin", "admin" },
            //        { "bd886912-c53a-4280-8709-8785e023f6c5", null, "tenant", "tenant" },
            //        { "efb97baa-d40f-4f4c-91b8-000323360d20", null, "owner", "owner" }
            //    });

            migrationBuilder.AddForeignKey(
                name: "FK_RoomMeters_Meters_MeterId",
                table: "RoomMeters",
                column: "MeterId",
                principalTable: "Meters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomMeters_Rooms_RoomId",
                table: "RoomMeters",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");
        }
    }
}
