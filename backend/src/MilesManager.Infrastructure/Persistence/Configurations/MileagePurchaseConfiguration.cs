using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MilesManager.Domain.Programs;

namespace MilesManager.Infrastructure.Persistence.Configurations;

internal sealed class MileagePurchaseConfiguration : IEntityTypeConfiguration<MileagePurchase>
{
    public void Configure(EntityTypeBuilder<MileagePurchase> builder)
    {
        builder.ToTable("mileage_purchases");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Partner).HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.AmountPaid).HasPrecision(18, 2);
        builder.Property(x => x.MilesPurchased).HasPrecision(18, 2);
        builder.Property(x => x.PurchaseDate).HasColumnType("date");
        builder.Property(x => x.PurchaseStatus).IsRequired();
        builder.HasOne(x => x.Program)
            .WithMany(p => p.MileagePurchases)
            .HasForeignKey(x => x.ProgramId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
