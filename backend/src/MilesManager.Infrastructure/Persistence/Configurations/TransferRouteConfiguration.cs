using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MilesManager.Domain.Transfers;

namespace MilesManager.Infrastructure.Persistence.Configurations;

internal sealed class TransferRouteConfiguration : IEntityTypeConfiguration<TransferRoute>
{
    public void Configure(EntityTypeBuilder<TransferRoute> builder)
    {
        builder.ToTable("transfer_routes");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.OriginProgramId, x.DestinationProgramId }).IsUnique();
        builder.Property(x => x.FromAmount).HasPrecision(18, 4);
        builder.Property(x => x.ToAmount).HasPrecision(18, 4);
        builder.HasOne(x => x.OriginProgram)
            .WithMany(p => p.OriginTransfers)
            .HasForeignKey(x => x.OriginProgramId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.DestinationProgram)
            .WithMany(p => p.DestinationTransfers)
            .HasForeignKey(x => x.DestinationProgramId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
