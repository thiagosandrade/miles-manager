using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilesManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveColumnLoyaltyProgram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationAfterMonths",
                table: "programs");

            migrationBuilder.DropColumn(
                name: "ExpirationEnabled",
                table: "programs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "programs");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "programs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "programs");

            migrationBuilder.AddColumn<int>(
                name: "ExpirationAfterMonths",
                table: "programs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ExpirationEnabled",
                table: "programs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "programs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
