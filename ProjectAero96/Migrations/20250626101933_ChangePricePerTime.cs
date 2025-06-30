using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAero96.Migrations
{
    /// <inheritdoc />
    public partial class ChangePricePerTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PricePerTime",
                table: "AirplaneModels",
                newName: "PricePerHour");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PricePerHour",
                table: "AirplaneModels",
                newName: "PricePerTime");
        }
    }
}
