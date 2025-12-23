using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Constants;
using SleshWrites.Domain.ValueObjects;

namespace SleshWrites.Infrastructure.Persistence.Configurations;

/// <summary>
/// Extension methods for common entity configuration patterns.
/// Reduces duplication across entity configurations.
/// </summary>
public static class EntityConfigurationExtensions
{
    /// <summary>
    /// Configures a Slug owned type with standard settings.
    /// </summary>
    public static void ConfigureSlug<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, Slug?>> slugExpression)
        where TEntity : class
    {
        builder.OwnsOne(slugExpression, slugBuilder =>
        {
            slugBuilder.Property(s => s.Value)
                .HasColumnName("Slug")
                .IsRequired()
                .HasMaxLength(ValidationConstants.Slug.MaxLength);

            slugBuilder.HasIndex(s => s.Value)
                .IsUnique();
        });
    }

    /// <summary>
    /// Ignores domain events from persistence (they are dispatched separately).
    /// </summary>
    public static void IgnoreDomainEvents<TEntity>(
        this EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity
    {
        builder.Ignore(e => e.DomainEvents);
    }

    /// <summary>
    /// Configures common audit timestamp properties.
    /// </summary>
    public static void ConfigureAuditTimestamps<TEntity>(
        this EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity
    {
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();
    }
}
