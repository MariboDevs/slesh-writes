namespace SleshWrites.API.Contracts.Categories;

/// <summary>
/// Request model for creating a new category.
/// </summary>
public sealed record CreateCategoryRequest(
    string Name,
    string? Description,
    int DisplayOrder);
