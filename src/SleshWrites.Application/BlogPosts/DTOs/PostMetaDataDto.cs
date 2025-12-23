namespace SleshWrites.Application.BlogPosts.DTOs;

/// <summary>
/// Data transfer object for post SEO metadata.
/// </summary>
public sealed record PostMetaDataDto(
    string? Title,
    string? Description,
    string? Keywords,
    string? CanonicalUrl,
    string? OgImage);
