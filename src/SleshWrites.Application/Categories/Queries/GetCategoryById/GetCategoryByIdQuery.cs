using SleshWrites.Application.Categories.DTOs;
using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.Categories.Queries.GetCategoryById;

/// <summary>
/// Query to get a category by ID.
/// </summary>
public sealed record GetCategoryByIdQuery(Guid Id) : IQuery<CategoryDto>;
