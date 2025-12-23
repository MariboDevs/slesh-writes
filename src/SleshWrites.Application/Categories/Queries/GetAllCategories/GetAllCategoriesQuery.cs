using SleshWrites.Application.Categories.DTOs;
using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.Categories.Queries.GetAllCategories;

/// <summary>
/// Query to get all categories.
/// </summary>
public sealed record GetAllCategoriesQuery : IQuery<IReadOnlyList<CategoryDto>>;
