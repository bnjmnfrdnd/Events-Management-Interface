using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventsManagementInterface.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateAttendeeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DrinkTokenAllowance",
                table: "Attendee",
                newName: "NonAlcoholicDrinkTokenAllowance");

            migrationBuilder.AddColumn<int>(
                name: "AlcoholicDrinkTokenAllowance",
                table: "Attendee",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "VendorInput",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuestIdentificationNumber = table.Column<int>(type: "int", nullable: false),
                    AlcoholicDrinkToken = table.Column<int>(type: "int", nullable: false),
                    NonAlcoholicDrinkToken = table.Column<int>(type: "int", nullable: false),
                    FoodToken = table.Column<int>(type: "int", nullable: false),
                    VendorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorInput", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorInput");

            migrationBuilder.DropColumn(
                name: "AlcoholicDrinkTokenAllowance",
                table: "Attendee");

            migrationBuilder.RenameColumn(
                name: "NonAlcoholicDrinkTokenAllowance",
                table: "Attendee",
                newName: "DrinkTokenAllowance");
        }
    }
}
