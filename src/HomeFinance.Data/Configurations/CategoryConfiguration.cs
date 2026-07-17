using HomeFinance.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeFinance.Data.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(64);
        builder.HasIndex(c => c.Name).IsUnique();
        builder.Property(c => c.ColorHex).IsRequired().HasMaxLength(7);
        builder.Property(c => c.Icon).HasMaxLength(128);
        builder.HasMany(c => c.Transactions).WithOne(t => t.Category)
            .HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.Restrict);

        builder.Metadata.FindNavigation(nameof(Category.Transactions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
