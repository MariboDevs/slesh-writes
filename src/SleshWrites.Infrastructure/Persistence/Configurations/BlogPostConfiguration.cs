using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SleshWrites.Domain.Entities;

namespace SleshWrites.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for BlogPost entity.
/// </summary>
public sealed class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        builder.ToTable("BlogPosts");

        builder.HasKey(bp => bp.Id);

        builder.Property(bp => bp.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.ConfigureSlug(bp => bp.Slug);

        builder.Property(bp => bp.Content)
            .IsRequired();

        builder.Property(bp => bp.Excerpt)
            .HasMaxLength(500);

        builder.Property(bp => bp.FeaturedImage)
            .HasMaxLength(2048);

        builder.Property(bp => bp.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(bp => bp.PublishedAt);

        builder.Property(bp => bp.AuthorId)
            .IsRequired();

        builder.Property(bp => bp.CategoryId)
            .IsRequired();

        builder.OwnsOne(bp => bp.MetaData, metaBuilder =>
        {
            metaBuilder.Property(m => m.Title)
                .HasColumnName("MetaTitle")
                .HasMaxLength(60);

            metaBuilder.Property(m => m.Description)
                .HasColumnName("MetaDescription")
                .HasMaxLength(160);

            metaBuilder.Property(m => m.Keywords)
                .HasColumnName("MetaKeywords")
                .HasMaxLength(200);

            metaBuilder.Property(m => m.CanonicalUrl)
                .HasColumnName("CanonicalUrl")
                .HasMaxLength(2048);

            metaBuilder.Property(m => m.OgImage)
                .HasColumnName("OgImage")
                .HasMaxLength(2048);
        });

        builder.ConfigureAuditTimestamps();

        // Many-to-many relationship with Tags
        builder.HasMany(bp => bp.Tags)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "BlogPostTag",
                j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<BlogPost>().WithMany().HasForeignKey("BlogPostId").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("BlogPostId", "TagId");
                    j.ToTable("BlogPostTags");
                });

        // Relationship with Category
        builder.HasOne(bp => bp.Category)
            .WithMany()
            .HasForeignKey(bp => bp.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index for querying by status and published date
        builder.HasIndex(bp => new { bp.Status, bp.PublishedAt })
            .HasDatabaseName("IX_BlogPosts_Status_PublishedAt");

        builder.HasIndex(bp => bp.AuthorId)
            .HasDatabaseName("IX_BlogPosts_AuthorId");

        builder.HasIndex(bp => bp.CategoryId)
            .HasDatabaseName("IX_BlogPosts_CategoryId");

        builder.IgnoreDomainEvents();
    }
}
