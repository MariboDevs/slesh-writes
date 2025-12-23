namespace SleshWrites.Application.Authors.DTOs;

/// <summary>
/// Data transfer object for an author.
/// </summary>
public sealed record AuthorDto(
    Guid Id,
    string AzureAdB2CId,
    string DisplayName,
    string? Email,
    string? Bio,
    string? AvatarUrl,
    IReadOnlyDictionary<string, string> SocialLinks,
    DateTime CreatedAt,
    DateTime UpdatedAt);
