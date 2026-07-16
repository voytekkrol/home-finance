using HomeFinance.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeFinance.Data.Configurations;

internal sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Amount).HasColumnType("decimal(18,2)");
        builder.Property(t => t.Description).IsRequired().HasMaxLength(256);
        builder.HasIndex(t => t.OccurredOn);
        builder.HasIndex(t => t.CategoryId);
        builder.HasIndex(t => t.AccountId);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(t => t.EnteredByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.ToTable(t => t.HasCheckConstraint("CK_Transaction_AmountNonZero", "\"Amount\" <> 0"));
    }
}
