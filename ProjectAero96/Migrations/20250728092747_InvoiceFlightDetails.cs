using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAero96.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceFlightDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArrivalCity",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DepartureCity",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FlightDuration",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrivalCity",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DepartureCity",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "FlightDuration",
                table: "Invoices");
        }
    }
}
