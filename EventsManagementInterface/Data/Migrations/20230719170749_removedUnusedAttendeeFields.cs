using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventsManagementInterface.Data.Migrations
{
    /// <inheritdoc />
    public partial class removedUnusedAttendeeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlcoholicDrinkTokensUsed",
                table: "Attendee");

            migrationBuilder.DropColumn(
                name: "FoodTokensUsed",
                table: "Attendee");

            migrationBuilder.DropColumn(
                name: "NonAlcoholicDrinkTokensUsed",
                table: "Attendee");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AlcoholicDrinkTokensUsed",
                table: "Attendee",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FoodTokensUsed",
                table: "Attendee",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NonAlcoholicDrinkTokensUsed",
                table: "Attendee",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
