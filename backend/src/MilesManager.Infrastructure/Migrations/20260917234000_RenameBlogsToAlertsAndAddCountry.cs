using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilesManager.Infrastructure.Migrations;

public partial class RenameBlogsToAlertsAndAddCountry : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameTable(name: "blog_sources", newName: "alert_sources");
        migrationBuilder.RenameTable(name: "blog_items", newName: "alert_items");

        migrationBuilder.RenameIndex(
            name: "IX_blog_sources_ExternalUrl",
            table: "alert_sources",
            newName: "IX_alert_sources_ExternalUrl");

        migrationBuilder.RenameIndex(
            name: "IX_blog_items_BlogItemPublishedDate",
            table: "alert_items",
            newName: "IX_alert_items_BlogItemPublishedDate");

        migrationBuilder.RenameIndex(
            name: "IX_blog_items_ExternalUrl",
            table: "alert_items",
            newName: "IX_alert_items_ExternalUrl");

        migrationBuilder.AddColumn<string>(
            name: "Country",
            table: "alert_sources",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Country",
            table: "alert_items",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Country", table: "alert_sources");
        migrationBuilder.DropColumn(name: "Country", table: "alert_items");

        migrationBuilder.RenameIndex(
            name: "IX_alert_sources_ExternalUrl",
            table: "alert_sources",
            newName: "IX_blog_sources_ExternalUrl");

        migrationBuilder.RenameIndex(
            name: "IX_alert_items_BlogItemPublishedDate",
            table: "alert_items",
            newName: "IX_blog_items_BlogItemPublishedDate");

        migrationBuilder.RenameIndex(
            name: "IX_alert_items_ExternalUrl",
            table: "alert_items",
            newName: "IX_blog_items_ExternalUrl");

        migrationBuilder.RenameTable(name: "alert_sources", newName: "blog_sources");
        migrationBuilder.RenameTable(name: "alert_items", newName: "blog_items");
    }
}
