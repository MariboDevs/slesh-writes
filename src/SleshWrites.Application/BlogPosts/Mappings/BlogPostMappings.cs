using SleshWrites.Application.BlogPosts.DTOs;
using SleshWrites.Domain.Entities;

namespace SleshWrites.Application.BlogPosts.Mappings;

/// <summary>
/// Extension methods for mapping domain entities to DTOs.
/// </summary>
public static class BlogPostMappings
{
    public static BlogPostDto ToDto(this BlogPost post, string? authorName = null, string? categoryName = null)
    {
        return new BlogPostDto(
            Id: post.Id,
            Title: post.Title,
            Slug: post.Slug.Value,
            Content: post.Content,
            Excerpt: post.Excerpt,
            FeaturedImage: post.FeaturedImage,
            Status: post.Status,
            PublishedAt: post.PublishedAt,
            CreatedAt: post.CreatedAt,
            UpdatedAt: post.UpdatedAt,
            AuthorId: post.AuthorId,
            AuthorName: authorName,
            CategoryId: post.CategoryId,
            CategoryName: categoryName,
            MetaData: post.MetaData.ToDto(),
            Tags: post.Tags.Select(t => t.ToDto()).ToList());
    }

    public static BlogPostSummaryDto ToSummaryDto(this BlogPost post, string? authorName = null, string? categoryName = null)
    {
        return new BlogPostSummaryDto(
            Id: post.Id,
            Title: post.Title,
            Slug: post.Slug.Value,
            Excerpt: post.Excerpt,
            FeaturedImage: post.FeaturedImage,
            Status: post.Status,
            PublishedAt: post.PublishedAt,
            CreatedAt: post.CreatedAt,
            AuthorName: authorName,
            CategoryName: categoryName,
            TagNames: post.Tags.Select(t => t.Name).ToList());
    }

    public static PostMetaDataDto ToDto(this Domain.ValueObjects.PostMetaData metaData)
    {
        return new PostMetaDataDto(
            Title: metaData.Title,
            Description: metaData.Description,
            Keywords: metaData.Keywords,
            CanonicalUrl: metaData.CanonicalUrl,
            OgImage: metaData.OgImage);
    }

    public static TagDto ToDto(this Tag tag)
    {
        return new TagDto(
            Id: tag.Id,
            Name: tag.Name,
            Slug: tag.Slug.Value);
    }

    public static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto(
            Id: category.Id,
            Name: category.Name,
            Slug: category.Slug.Value,
            Description: category.Description,
            DisplayOrder: category.DisplayOrder);
    }
}
