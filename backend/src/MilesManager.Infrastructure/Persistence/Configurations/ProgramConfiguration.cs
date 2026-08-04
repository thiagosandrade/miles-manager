using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MilesManager.Domain.Programs;

namespace MilesManager.Infrastructure.Persistence.Configurations;

internal sealed class ProgramConfiguration : IEntityTypeConfiguration<Program>
{
    public void Configure(EntityTypeBuilder<Program> builder)
    {
        builder.ToTable("programs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ProgramType).IsRequired();
        builder.Property(x => x.Website).HasMaxLength(512);
        builder.Property(x => x.Logo).HasMaxLength(512);
        builder.Property(x => x.ReferencePricePerThousand).HasPrecision(18, 4);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
    }
}
