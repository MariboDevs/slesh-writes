using SleshWrites.Application.Tags.DTOs;
using SleshWrites.Domain.Entities;

namespace SleshWrites.Application.Tags.Mappings;

/// <summary>
/// Extension methods for mapping Tag entities to DTOs.
/// </summary>
public static class TagMappings
{
    public static TagDto ToDto(this Tag tag) =>
        new(
            tag.Id,
            tag.Name,
            tag.Slug?.Value ?? string.Empty,
            tag.CreatedAt,
            tag.UpdatedAt);

    public static IReadOnlyList<TagDto> ToDtoList(this IEnumerable<Tag> tags) =>
        tags.Select(t => t.ToDto()).ToList();
}
