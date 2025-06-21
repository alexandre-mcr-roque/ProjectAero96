using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAero96.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelAirplane : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightStops_Cities_CityId",
                table: "FlightStops");

            migrationBuilder.RenameColumn(
                name: "BrandImageId",
                table: "Airplanes",
                newName: "AirlineImageId");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Airplanes",
                type: "nvarchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Airline",
                table: "Airplanes",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ModelName",
                table: "AirplaneModels",
                type: "nvarchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ModelNameShort",
                table: "AirplaneModels",
                type: "nvarchar(20)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FlightStops_Cities_CityId",
                table: "FlightStops",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightStops_Cities_CityId",
                table: "FlightStops");

            migrationBuilder.DropColumn(
                name: "Airline",
                table: "Airplanes");

            migrationBuilder.DropColumn(
                name: "ModelNameShort",
                table: "AirplaneModels");

            migrationBuilder.RenameColumn(
                name: "AirlineImageId",
                table: "Airplanes",
                newName: "BrandImageId");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Airplanes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModelName",
                table: "AirplaneModels",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightStops_Cities_CityId",
                table: "FlightStops",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
