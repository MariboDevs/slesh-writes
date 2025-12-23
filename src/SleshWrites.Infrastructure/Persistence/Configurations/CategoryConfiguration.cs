using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SleshWrites.Domain.Entities;

namespace SleshWrites.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Category entity.
/// </summary>
public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.OwnsOne(c => c.Slug, slugBuilder =>
        {
            slugBuilder.Property(s => s.Value)
                .HasColumnName("Slug")
                .IsRequired()
                .HasMaxLength(200);

            slugBuilder.HasIndex(s => s.Value)
                .IsUnique();
        });

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.DisplayOrder)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired();

        builder.HasIndex(c => c.DisplayOrder)
            .HasDatabaseName("IX_Categories_DisplayOrder");

        // Ignore domain events
        builder.Ignore(c => c.DomainEvents);
    }
}
