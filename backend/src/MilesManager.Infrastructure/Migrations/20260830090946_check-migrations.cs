using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilesManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Checkmigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "transfer_routes");

            migrationBuilder.DropColumn(
                name: "TransferFactor",
                table: "transfer_routes");

            migrationBuilder.AddColumn<decimal>(
                name: "FromAmount",
                table: "transfer_routes",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ToAmount",
                table: "transfer_routes",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromAmount",
                table: "transfer_routes");

            migrationBuilder.DropColumn(
                name: "ToAmount",
                table: "transfer_routes");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "transfer_routes",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TransferFactor",
                table: "transfer_routes",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
