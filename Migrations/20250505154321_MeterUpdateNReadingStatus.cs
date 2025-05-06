using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyProjectIT15.Migrations
{
    /// <inheritdoc />
    public partial class MeterUpdateNReadingStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "0241dca7-e90e-46b0-90d9-2377d8c1232e");

            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "9688a344-0fdb-40f2-8fea-c77ef3d0a385");

            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "bf133950-dbbc-42e2-980a-4a6c0f60fb83");

            migrationBuilder.AddColumn<string>(
                name: "MeterType",
                table: "Meters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MeterReadings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "WaterConsumption",
                table: "MeterReadings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WaterCurrentReading",
                table: "MeterReadings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WaterPreviousReading",
                table: "MeterReadings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            //migrationBuilder.InsertData(
            //    table: "AspNetRoles",
            //    columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
            //    values: new object[,]
            //    {
            //        { "2ac3464f-2311-4708-92a4-938049456156", null, "owner", "owner" },
            //        { "e6c5441f-5e47-44e4-aa14-27c2154e15f9", null, "admin", "admin" },
            //        { "f002d451-add2-4403-975c-28da65e7b2b3", null, "tenant", "tenant" }
            //    });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "2ac3464f-2311-4708-92a4-938049456156");

            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "e6c5441f-5e47-44e4-aa14-27c2154e15f9");

            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "f002d451-add2-4403-975c-28da65e7b2b3");

            migrationBuilder.DropColumn(
                name: "MeterType",
                table: "Meters");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MeterReadings");

            migrationBuilder.DropColumn(
                name: "WaterConsumption",
                table: "MeterReadings");

            migrationBuilder.DropColumn(
                name: "WaterCurrentReading",
                table: "MeterReadings");

            migrationBuilder.DropColumn(
                name: "WaterPreviousReading",
                table: "MeterReadings");

            //migrationBuilder.InsertData(
            //    table: "AspNetRoles",
            //    columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
            //    values: new object[,]
            //    {
            //        { "0241dca7-e90e-46b0-90d9-2377d8c1232e", null, "admin", "admin" },
            //        { "9688a344-0fdb-40f2-8fea-c77ef3d0a385", null, "owner", "owner" },
            //        { "bf133950-dbbc-42e2-980a-4a6c0f60fb83", null, "tenant", "tenant" }
            //    });
        }
    }
}
