using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilesManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeMileageAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mileage_accounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mileage_accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountNumber = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CurrentBalanceMiles = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ExpirationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PendingMiles = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mileage_accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mileage_accounts_programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mileage_accounts_ProgramId",
                table: "mileage_accounts",
                column: "ProgramId");
        }
    }
}
