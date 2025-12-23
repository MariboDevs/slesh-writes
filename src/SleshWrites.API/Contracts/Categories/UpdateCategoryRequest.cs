namespace SleshWrites.API.Contracts.Categories;

/// <summary>
/// Request model for updating an existing category.
/// </summary>
public sealed record UpdateCategoryRequest(
    string Name,
    string? Description,
    int DisplayOrder);
