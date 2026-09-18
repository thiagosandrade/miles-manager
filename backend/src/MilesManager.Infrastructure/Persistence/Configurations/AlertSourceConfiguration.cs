using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MilesManager.Domain.Alerts;

namespace MilesManager.Infrastructure.Persistence.Configurations;

internal sealed class AlertSourceConfiguration : IEntityTypeConfiguration<AlertSource>
{
    public void Configure(EntityTypeBuilder<AlertSource> builder)
    {
        builder.ToTable("alert_sources");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ExternalUrl).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.Country).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.ExternalUrl).IsUnique();
    }
}
