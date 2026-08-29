using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MilesManager.Domain.Redemptions;

namespace MilesManager.Infrastructure.Persistence.Configurations;

internal sealed class FlightRedemptionConfiguration : IEntityTypeConfiguration<FlightRedemption>
{
    public void Configure(EntityTypeBuilder<FlightRedemption> builder)
    {
        builder.ToTable("flight_redemptions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OriginAirportCode).HasMaxLength(3).IsRequired();
        builder.Property(x => x.DestinationAirportCode).HasMaxLength(3).IsRequired();
        builder.Property(x => x.FlightDate).HasColumnType("date");
        builder.Property(x => x.MilesUsed).HasPrecision(18, 2);
        builder.Property(x => x.Taxes).HasPrecision(18, 2);
        builder.Property(x => x.CashPrice).HasPrecision(18, 2);
        builder.HasOne(x => x.LoyaltyProgram)
            .WithMany(p => p.FlightRedemptions)
            .HasForeignKey(x => x.ProgramId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
