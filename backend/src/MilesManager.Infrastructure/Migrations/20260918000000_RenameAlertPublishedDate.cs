using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilesManager.Infrastructure.Migrations;

public partial class RenameAlertPublishedDate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "BlogItemPublishedDate",
            table: "alert_items",
            newName: "PublishedDate");

        migrationBuilder.RenameIndex(
            name: "IX_alert_items_BlogItemPublishedDate",
            table: "alert_items",
            newName: "IX_alert_items_PublishedDate");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameIndex(
            name: "IX_alert_items_PublishedDate",
            table: "alert_items",
            newName: "IX_alert_items_BlogItemPublishedDate");

        migrationBuilder.RenameColumn(
            name: "PublishedDate",
            table: "alert_items",
            newName: "BlogItemPublishedDate");
    }
}
