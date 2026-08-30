using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilesManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class renameProgramFactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transfer_routes");

            migrationBuilder.CreateTable(
                name: "program_factor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    DestinationProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromAmount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    ToAmount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_program_factor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_program_factor_programs_DestinationProgramId",
                        column: x => x.DestinationProgramId,
                        principalTable: "programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_program_factor_programs_OriginProgramId",
                        column: x => x.OriginProgramId,
                        principalTable: "programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_program_factor_DestinationProgramId",
                table: "program_factor",
                column: "DestinationProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_program_factor_OriginProgramId_DestinationProgramId",
                table: "program_factor",
                columns: new[] { "OriginProgramId", "DestinationProgramId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "program_factor");

            migrationBuilder.CreateTable(
                name: "transfer_routes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DestinationProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FromAmount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    ToAmount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transfer_routes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_transfer_routes_programs_DestinationProgramId",
                        column: x => x.DestinationProgramId,
                        principalTable: "programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transfer_routes_programs_OriginProgramId",
                        column: x => x.OriginProgramId,
                        principalTable: "programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_transfer_routes_DestinationProgramId",
                table: "transfer_routes",
                column: "DestinationProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_transfer_routes_OriginProgramId_DestinationProgramId",
                table: "transfer_routes",
                columns: new[] { "OriginProgramId", "DestinationProgramId" },
                unique: true);
        }
    }
}
