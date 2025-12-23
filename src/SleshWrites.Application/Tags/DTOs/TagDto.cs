namespace SleshWrites.Application.Tags.DTOs;

/// <summary>
/// Data transfer object for a tag.
/// </summary>
public sealed record TagDto(
    Guid Id,
    string Name,
    string Slug,
    DateTime CreatedAt,
    DateTime UpdatedAt);
