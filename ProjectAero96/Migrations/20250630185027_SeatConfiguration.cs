using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAero96.Migrations
{
    /// <inheritdoc />
    public partial class SeatConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ESeats",
                table: "Airplanes");

            migrationBuilder.RenameColumn(
                name: "FCSeats",
                table: "Airplanes",
                newName: "SeatRows");

            migrationBuilder.AddColumn<byte>(
                name: "SeatColumns",
                table: "Airplanes",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "WindowSeats",
                table: "Airplanes",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "SeatColumns",
                table: "AirplaneModels",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "SeatRows",
                table: "AirplaneModels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "WindowSeats",
                table: "AirplaneModels",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatColumns",
                table: "Airplanes");

            migrationBuilder.DropColumn(
                name: "WindowSeats",
                table: "Airplanes");

            migrationBuilder.DropColumn(
                name: "SeatColumns",
                table: "AirplaneModels");

            migrationBuilder.DropColumn(
                name: "SeatRows",
                table: "AirplaneModels");

            migrationBuilder.DropColumn(
                name: "WindowSeats",
                table: "AirplaneModels");

            migrationBuilder.RenameColumn(
                name: "SeatRows",
                table: "Airplanes",
                newName: "FCSeats");

            migrationBuilder.AddColumn<int>(
                name: "ESeats",
                table: "Airplanes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
