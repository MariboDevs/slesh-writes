using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SleshWrites.Domain.Constants;
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
            .HasMaxLength(ValidationConstants.Tag.NameMaxLength);

        builder.ConfigureSlug(t => t.Slug);

        builder.ConfigureAuditTimestamps();

        builder.IgnoreDomainEvents();
    }
}
