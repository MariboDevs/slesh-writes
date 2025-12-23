namespace SleshWrites.Application.Categories.DTOs;

/// <summary>
/// Data transfer object for a category.
/// </summary>
public sealed record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    int DisplayOrder,
    DateTime CreatedAt,
    DateTime UpdatedAt);
