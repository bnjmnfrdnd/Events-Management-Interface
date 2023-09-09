using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventsManagementInterface.Data.Migrations
{
    /// <inheritdoc />
    public partial class orderVendorUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DrinkName",
                table: "VendorInput");

            migrationBuilder.DropColumn(
                name: "DrinkPrice",
                table: "VendorInput");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "VendorInput");

            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "VendorInput",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "VendorInput",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "GuestIdentificationNumber",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archived",
                table: "VendorInput");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "VendorInput");

            migrationBuilder.DropColumn(
                name: "GuestIdentificationNumber",
                table: "Order");

            migrationBuilder.AddColumn<string>(
                name: "DrinkName",
                table: "VendorInput",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DrinkPrice",
                table: "VendorInput",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "VendorInput",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
