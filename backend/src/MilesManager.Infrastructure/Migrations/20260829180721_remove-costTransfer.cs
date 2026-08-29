using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilesManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removecostTransfer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostPerThousandDestination",
                table: "mileage_transfers");

            migrationBuilder.DropColumn(
                name: "CostPerThousandOrigin",
                table: "mileage_transfers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostPerThousandDestination",
                table: "mileage_transfers",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CostPerThousandOrigin",
                table: "mileage_transfers",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
