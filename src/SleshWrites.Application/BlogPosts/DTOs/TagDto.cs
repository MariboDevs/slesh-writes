namespace SleshWrites.Application.BlogPosts.DTOs;

/// <summary>
/// Data transfer object for a tag.
/// </summary>
public sealed record TagDto(
    Guid Id,
    string Name,
    string Slug);
