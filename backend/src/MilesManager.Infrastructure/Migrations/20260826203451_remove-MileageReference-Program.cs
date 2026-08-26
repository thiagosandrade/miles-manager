using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilesManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeMileageReferenceProgram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferencePricePerThousand",
                table: "programs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ReferencePricePerThousand",
                table: "programs",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
