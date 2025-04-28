using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CosmoCargo.Migrations
{
    /// <inheritdoc />
    public partial class UpdateShipmentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "risk_level",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "scheduled_date",
                table: "shipments");

            migrationBuilder.RenameColumn(
                name: "origin",
                table: "shipments",
                newName: "category");

            migrationBuilder.RenameColumn(
                name: "destination",
                table: "shipments",
                newName: "Receiver_Station");

            migrationBuilder.RenameColumn(
                name: "cargo",
                table: "shipments",
                newName: "Receiver_Planet");

            migrationBuilder.AddColumn<string>(
                name: "Receiver_Email",
                table: "shipments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Receiver_Name",
                table: "shipments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "shipments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "has_insurance",
                table: "shipments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "shipment_contact",
                columns: table => new
                {
                    ShipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    planet = table.Column<string>(type: "text", nullable: false),
                    station = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipment_contact", x => x.ShipmentId);
                    table.ForeignKey(
                        name: "FK_shipment_contact_shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "shipments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "shipment_contact");

            migrationBuilder.DropColumn(
                name: "Receiver_Email",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "Receiver_Name",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "description",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "has_insurance",
                table: "shipments");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "shipments",
                newName: "origin");

            migrationBuilder.RenameColumn(
                name: "Receiver_Station",
                table: "shipments",
                newName: "destination");

            migrationBuilder.RenameColumn(
                name: "Receiver_Planet",
                table: "shipments",
                newName: "cargo");

            migrationBuilder.AddColumn<int>(
                name: "risk_level",
                table: "shipments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "scheduled_date",
                table: "shipments",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
