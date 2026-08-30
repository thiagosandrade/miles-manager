using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilesManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTransferFactorToRatio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransferFactor",
                table: "transfer_routes",
                newName: "FromAmount");

            migrationBuilder.AlterColumn<decimal>(
                name: "FromAmount",
                table: "transfer_routes",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AddColumn<decimal>(
                name: "ToAmount",
                table: "transfer_routes",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.Sql(
                "UPDATE transfer_routes SET \"ToAmount\" = 1.0 WHERE \"ToAmount\" IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ToAmount",
                table: "transfer_routes");

            migrationBuilder.AlterColumn<decimal>(
                name: "FromAmount",
                table: "transfer_routes",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.RenameColumn(
                name: "FromAmount",
                table: "transfer_routes",
                newName: "TransferFactor");
        }
    }
}
