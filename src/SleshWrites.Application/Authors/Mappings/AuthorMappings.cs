using SleshWrites.Application.Authors.DTOs;
using SleshWrites.Domain.Entities;

namespace SleshWrites.Application.Authors.Mappings;

/// <summary>
/// Extension methods for mapping Author entities to DTOs.
/// </summary>
public static class AuthorMappings
{
    public static AuthorDto ToDto(this Author author) =>
        new(
            author.Id,
            author.AzureAdB2CId,
            author.DisplayName,
            author.Email,
            author.Bio,
            author.AvatarUrl,
            author.SocialLinks,
            author.CreatedAt,
            author.UpdatedAt);

    public static IReadOnlyList<AuthorDto> ToDtoList(this IEnumerable<Author> authors) =>
        authors.Select(a => a.ToDto()).ToList();
}
