using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MilesManager.Domain.Alerts;

namespace MilesManager.Infrastructure.Persistence.Configurations;

internal sealed class AlertItemConfiguration : IEntityTypeConfiguration<AlertItem>
{
    public void Configure(EntityTypeBuilder<AlertItem> builder)
    {
        builder.ToTable("alert_items");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(500).IsRequired();
        builder.Property(x => x.ExternalUrl).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.Chips).HasMaxLength(2000);
        builder.Property(x => x.Country).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.ExternalUrl).IsUnique();
        builder.HasIndex(x => x.PublishedDate);
    }
}
