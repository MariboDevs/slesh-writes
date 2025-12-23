using SleshWrites.Domain.Enums;

namespace SleshWrites.Application.BlogPosts.DTOs;

/// <summary>
/// Data transfer object for a blog post.
/// </summary>
public sealed record BlogPostDto(
    Guid Id,
    string Title,
    string Slug,
    string Content,
    string? Excerpt,
    string? FeaturedImage,
    PostStatus Status,
    DateTime? PublishedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    Guid AuthorId,
    string? AuthorName,
    Guid CategoryId,
    string? CategoryName,
    PostMetaDataDto MetaData,
    IReadOnlyList<TagDto> Tags);

/// <summary>
/// Summary DTO for blog post listings.
/// </summary>
public sealed record BlogPostSummaryDto(
    Guid Id,
    string Title,
    string Slug,
    string? Excerpt,
    string? FeaturedImage,
    PostStatus Status,
    DateTime? PublishedAt,
    DateTime CreatedAt,
    string? AuthorName,
    string? CategoryName,
    IReadOnlyList<string> TagNames);
