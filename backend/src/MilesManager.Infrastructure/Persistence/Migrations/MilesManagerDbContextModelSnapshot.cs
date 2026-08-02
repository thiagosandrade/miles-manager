using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace MilesManager.Infrastructure.Persistence.Migrations;

[DbContext(typeof(MilesManagerDbContext))]
partial class MilesManagerDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.10")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        modelBuilder.Entity("MilesManager.Domain.Trips.Trip", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid");
            b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("timestamp with time zone");
            b.Property<decimal>("DistanceMiles").HasColumnType("numeric(10,2)");
            b.Property<string>("Name").IsRequired().HasMaxLength(200).HasColumnType("character varying(200)");
            b.Property<DateTimeOffset?>("UpdatedAtUtc").HasColumnType("timestamp with time zone");
            b.Property<DateOnly>("TripDate").HasColumnType("date");
            b.HasKey("Id");
            b.ToTable("trips");
        });

        modelBuilder.Entity("MilesManager.Domain.Users.AppUser", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid");
            b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("timestamp with time zone");
            b.Property<string>("Email").IsRequired().HasMaxLength(256).HasColumnType("character varying(256)");
            b.Property<bool>("IsActive").HasColumnType("boolean").HasDefaultValue(true);
            b.Property<DateTimeOffset?>("LastLoginAtUtc").HasColumnType("timestamp with time zone");
            b.Property<string>("FullName").IsRequired().HasMaxLength(200).HasColumnType("character varying(200)");
            b.Property<string>("PasswordHash").IsRequired().HasMaxLength(512).HasColumnType("character varying(512)");
            b.Property<int>("TokenVersion").HasColumnType("integer").HasDefaultValue(0);
            b.Property<DateTimeOffset?>("UpdatedAtUtc").HasColumnType("timestamp with time zone");
            b.HasKey("Id");
            b.HasIndex("Email").IsUnique();
            b.ToTable("users");
        });

        modelBuilder.Entity("MilesManager.Domain.Users.UserPreferences", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid");
            b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("timestamp with time zone");
            b.Property<bool>("EmailNotificationsEnabled").HasColumnType("boolean");
            b.Property<bool>("ExpirationAlertsEnabled").HasColumnType("boolean");
            b.Property<Guid>("UserId").HasColumnType("uuid");
            b.Property<string>("PreferredCurrency").IsRequired().HasMaxLength(3).HasColumnType("character varying(3)");
            b.Property<string>("Theme").HasMaxLength(24).HasColumnType("character varying(24)");
            b.Property<DateTimeOffset?>("UpdatedAtUtc").HasColumnType("timestamp with time zone");
            b.HasKey("Id");
            b.HasIndex("UserId").IsUnique();
            b.ToTable("user_preferences");
        });
    }
}
