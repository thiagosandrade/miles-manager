using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MilesManager.Domain.Partners;

namespace MilesManager.Infrastructure.Persistence.Configurations;

internal sealed class PartnerConfiguration : IEntityTypeConfiguration<Partner>
{
    public void Configure(EntityTypeBuilder<Partner> builder)
    {
        builder.ToTable("partners");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.WebsiteUrl).HasMaxLength(512);
        builder.Property(x => x.IataCode).HasMaxLength(8);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
    }
}
