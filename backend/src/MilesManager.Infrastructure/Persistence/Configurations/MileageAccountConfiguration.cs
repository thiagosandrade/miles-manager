using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MilesManager.Domain.Accounts;

namespace MilesManager.Infrastructure.Persistence.Configurations;

internal sealed class MileageAccountConfiguration : IEntityTypeConfiguration<MileageAccount>
{
    public void Configure(EntityTypeBuilder<MileageAccount> builder)
    {
        builder.ToTable("mileage_accounts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProgramId).IsRequired();
        builder.Property(x => x.UserId);
        builder.Property(x => x.AccountNumber).HasMaxLength(128).IsRequired();
        builder.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.CurrentBalanceMiles).HasPrecision(18, 2);
        builder.Property(x => x.PendingMiles).HasPrecision(18, 2);
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.ExpirationDate).HasColumnType("date");

        builder.HasOne<MilesManager.Domain.Programs.LoyaltyProgram>()
            .WithMany()
            .HasForeignKey(x => x.ProgramId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
