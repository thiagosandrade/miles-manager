using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MilesManager.Domain.Transactions;

namespace MilesManager.Infrastructure.Persistence.Configurations;

internal sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProgramId).IsRequired();
        builder.Property(x => x.AccountId);
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.Miles).HasPrecision(18, 2);
        builder.Property(x => x.OccurredAtUtc).HasColumnType("timestamp with time zone");
        builder.Property(x => x.Reference).HasMaxLength(256);
        builder.Property(x => x.Notes).HasMaxLength(1024);

        // Configure TPH (table-per-hierarchy) using discriminator
        builder.HasDiscriminator<string>("TransactionType")
            .HasValue<EarnTransaction>(nameof(EarnTransaction))
            .HasValue<RedeemTransaction>(nameof(RedeemTransaction))
            .HasValue<ExpirationTransaction>(nameof(ExpirationTransaction))
            .HasValue<TransferTransaction>(nameof(TransferTransaction));
    }
}
