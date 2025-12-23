using SleshWrites.Application.Categories.DTOs;
using SleshWrites.Domain.Entities;

namespace SleshWrites.Application.Categories.Mappings;

/// <summary>
/// Extension methods for mapping Category entities to DTOs.
/// </summary>
public static class CategoryMappings
{
    public static CategoryDto ToDto(this Category category) =>
        new(
            category.Id,
            category.Name,
            category.Slug?.Value ?? string.Empty,
            category.Description,
            category.DisplayOrder,
            category.CreatedAt,
            category.UpdatedAt);

    public static IReadOnlyList<CategoryDto> ToDtoList(this IEnumerable<Category> categories) =>
        categories.Select(c => c.ToDto()).ToList();
}
