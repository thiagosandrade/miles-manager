using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MilesManager.Domain.Programs;

namespace MilesManager.Infrastructure.Persistence.Configurations;

internal sealed class ProgramBalanceConfiguration : IEntityTypeConfiguration<ProgramBalance>
{
    public void Configure(EntityTypeBuilder<ProgramBalance> builder)
    {
        builder.ToTable("program_balances");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.UserId, x.ProgramId }).IsUnique();
        builder.Property(x => x.Balance).HasPrecision(18, 2);
        builder.HasOne(x => x.Program)
            .WithMany(p => p.ProgramBalances)
            .HasForeignKey(x => x.ProgramId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
