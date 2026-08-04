using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MilesManager.Domain.Pricing;

namespace MilesManager.Infrastructure.Persistence.Configurations;

internal sealed class ReferenceMileagePriceConfiguration : IEntityTypeConfiguration<ReferenceMileagePrice>
{
    public void Configure(EntityTypeBuilder<ReferenceMileagePrice> builder)
    {
        builder.ToTable("reference_mileage_prices");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PricePerThousand).HasPrecision(18, 4);
        builder.Property(x => x.EffectiveDate).HasColumnType("date");
        builder.HasIndex(x => new { x.ProgramId, x.EffectiveDate });
        builder.HasOne(x => x.Program)
            .WithMany(p => p.ReferenceMileagePrices)
            .HasForeignKey(x => x.ProgramId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
