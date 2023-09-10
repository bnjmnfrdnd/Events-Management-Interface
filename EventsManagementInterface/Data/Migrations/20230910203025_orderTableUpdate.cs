using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventsManagementInterface.Data.Migrations
{
    /// <inheritdoc />
    public partial class orderTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Order");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Order");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Size",
                table: "Order",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
