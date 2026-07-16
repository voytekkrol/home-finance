using HomeFinance.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeFinance.Data.Configurations;

internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).IsRequired().HasMaxLength(64);
        builder.HasIndex(a => a.Name).IsUnique();
        builder.Property(a => a.Currency).IsRequired().HasMaxLength(3).IsFixedLength();
        builder.Property(a => a.OpeningBalance).HasColumnType("decimal(18,2)");
        builder.Property(a => a.Type).HasConversion<int>();
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(a => a.OwnerUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(a => a.Transactions).WithOne(t => t.Account)
            .HasForeignKey(t => t.AccountId).OnDelete(DeleteBehavior.Restrict);

        // Rich domain model: navigation collection backed by private field
        builder.Metadata.FindNavigation(nameof(Account.Transactions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
