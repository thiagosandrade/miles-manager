using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilesManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fixdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
    DROP TABLE IF EXISTS "opportunities";
    """);

            migrationBuilder.Sql("""
    DROP TABLE IF EXISTS "opportunity_sources";
    """);

            migrationBuilder.Sql("""
    CREATE TABLE IF NOT EXISTS "alert_items"
    (
        "Id" uuid NOT NULL,
        "Title" character varying(500) NOT NULL,
        "ExternalUrl" character varying(2000) NOT NULL,
        "PublishedDate" timestamp with time zone NULL,
        "Chips" character varying(2000) NULL,
        "Country" character varying(100) NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone NULL,
        CONSTRAINT "PK_alert_items" PRIMARY KEY ("Id")
    );
    """);

            migrationBuilder.Sql("""
    CREATE TABLE IF NOT EXISTS "alert_sources"
    (
        "Id" uuid NOT NULL,
        "Title" character varying(200) NOT NULL,
        "ExternalUrl" character varying(2000) NOT NULL,
        "Country" character varying(100) NOT NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone NULL,
        CONSTRAINT "PK_alert_sources" PRIMARY KEY ("Id")
    );
    """);

            migrationBuilder.Sql("""
    CREATE UNIQUE INDEX IF NOT EXISTS "IX_alert_items_ExternalUrl"
        ON "alert_items" ("ExternalUrl");

    CREATE INDEX IF NOT EXISTS "IX_alert_items_PublishedDate"
        ON "alert_items" ("PublishedDate");

    CREATE UNIQUE INDEX IF NOT EXISTS "IX_alert_sources_ExternalUrl"
        ON "alert_sources" ("ExternalUrl");
    """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
