using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilesManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOpportunities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "opportunity_sources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Provider = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SourceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Configuration = table.Column<string>(type: "character varying(20000)", maxLength: 20000, nullable: true),
                    LastIngestionAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSuccessfulIngestionAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastError = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    OpportunitiesDiscovered = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opportunity_sources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "opportunities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SourceProgram = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DestinationProgram = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Benefit = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Conditions = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExternalUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    OriginalContent = table.Column<string>(type: "character varying(50000)", maxLength: 50000, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DiscoveredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OpportunitySourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opportunities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_opportunities_opportunity_sources_OpportunitySourceId",
                        column: x => x.OpportunitySourceId,
                        principalTable: "opportunity_sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_opportunities_ExternalUrl",
                table: "opportunities",
                column: "ExternalUrl");

            migrationBuilder.CreateIndex(
                name: "IX_opportunities_OpportunitySourceId_Status",
                table: "opportunities",
                columns: new[] { "OpportunitySourceId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_opportunity_sources_Enabled",
                table: "opportunity_sources",
                column: "Enabled");

            migrationBuilder.CreateIndex(
                name: "IX_opportunity_sources_Provider_Name",
                table: "opportunity_sources",
                columns: new[] { "Provider", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "opportunities");

            migrationBuilder.DropTable(
                name: "opportunity_sources");
        }
    }
}
