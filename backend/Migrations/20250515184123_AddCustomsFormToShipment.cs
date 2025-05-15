using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CosmoCargo.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomsFormToShipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "customs_contains_lifeforms",
                table: "shipments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "customs_is_plasma_active",
                table: "shipments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "customs_lifeform_type",
                table: "shipments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "customs_notes",
                table: "shipments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "customs_origin_planet_laws_confirmed",
                table: "shipments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "customs_plasma_stability_level",
                table: "shipments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "customs_quarantine_required",
                table: "shipments",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "customs_contains_lifeforms",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "customs_is_plasma_active",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "customs_lifeform_type",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "customs_notes",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "customs_origin_planet_laws_confirmed",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "customs_plasma_stability_level",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "customs_quarantine_required",
                table: "shipments");
        }
    }
}
