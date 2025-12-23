using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SleshWrites.Domain.Entities;

namespace SleshWrites.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Tag entity.
/// </summary>
public sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.OwnsOne(t => t.Slug, slugBuilder =>
        {
            slugBuilder.Property(s => s.Value)
                .HasColumnName("Slug")
                .IsRequired()
                .HasMaxLength(200);

            slugBuilder.HasIndex(s => s.Value)
                .IsUnique();
        });

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .IsRequired();

        // Ignore domain events
        builder.Ignore(t => t.DomainEvents);
    }
}
