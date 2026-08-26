using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MilesManager.Infrastructure.Persistence.Configurations;

internal sealed class ProgramConfiguration : IEntityTypeConfiguration<Domain.Programs.LoyaltyProgram>
{
    public void Configure(EntityTypeBuilder<Domain.Programs.LoyaltyProgram> builder)
    {
        builder.ToTable("programs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProgramCode).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ProgramType).IsRequired();
        builder.Property(x => x.Website).HasMaxLength(512);
        builder.Property(x => x.Logo).HasMaxLength(512);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
    }
}
