using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SleshWrites.Domain.Entities;

namespace SleshWrites.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Author entity.
/// </summary>
public sealed class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("Authors");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AzureAdB2CId)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasIndex(a => a.AzureAdB2CId)
            .IsUnique()
            .HasDatabaseName("IX_Authors_AzureAdB2CId");

        builder.Property(a => a.DisplayName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Email)
            .HasMaxLength(256);

        builder.Property(a => a.Bio)
            .HasMaxLength(1000);

        builder.Property(a => a.AvatarUrl)
            .HasMaxLength(2048);

        // Store social links as JSON with proper value comparer
        var dictionaryComparer = new ValueComparer<IReadOnlyDictionary<string, string>>(
            (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToDictionary(k => k.Key, v => v.Value));

        builder.Property(a => a.SocialLinks)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, string>())
            .Metadata.SetValueComparer(dictionaryComparer);

        builder.Property(a => a.SocialLinks)
            .HasColumnType("nvarchar(max)");

        builder.ConfigureAuditTimestamps();

        builder.IgnoreDomainEvents();
    }
}
