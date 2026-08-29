using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MilesManager.Domain.Transfers;

namespace MilesManager.Infrastructure.Persistence.Configurations;

internal sealed class MileageTransferConfiguration : IEntityTypeConfiguration<MileageTransfer>
{
    public void Configure(EntityTypeBuilder<MileageTransfer> builder)
    {
        builder.ToTable("mileage_transfers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MilesSent).HasPrecision(18, 2);
        builder.Property(x => x.BonusPercentage).HasPrecision(5, 2);
        builder.Property(x => x.MilesReceived).HasPrecision(18, 2);
        builder.Property(x => x.TransferDate).HasColumnType("date");
        builder.HasOne(x => x.OriginProgram)
            .WithMany()
            .HasForeignKey(x => x.OriginProgramId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.DestinationProgram)
            .WithMany()
            .HasForeignKey(x => x.DestinationProgramId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
