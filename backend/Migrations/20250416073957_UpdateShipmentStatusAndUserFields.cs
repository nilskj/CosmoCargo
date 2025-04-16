using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CosmoCargo.Migrations
{
    /// <inheritdoc />
    public partial class UpdateShipmentStatusAndUserFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Shipments",
                newName: "Cargo");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledDate",
                table: "Shipments",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledDate",
                table: "Shipments");

            migrationBuilder.RenameColumn(
                name: "Cargo",
                table: "Shipments",
                newName: "Category");
        }
    }
}
