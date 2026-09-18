using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilesManager.Infrastructure.Migrations;

public partial class AddBlogAlerts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "blog_sources",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                ExternalUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_blog_sources", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "blog_items",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                ExternalUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                BlogItemPublishedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                Chips = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_blog_items", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_blog_items_BlogItemPublishedDate",
            table: "blog_items",
            column: "BlogItemPublishedDate");

        migrationBuilder.CreateIndex(
            name: "IX_blog_items_ExternalUrl",
            table: "blog_items",
            column: "ExternalUrl",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_blog_sources_ExternalUrl",
            table: "blog_sources",
            column: "ExternalUrl",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "blog_items");
        migrationBuilder.DropTable(name: "blog_sources");
    }
}
