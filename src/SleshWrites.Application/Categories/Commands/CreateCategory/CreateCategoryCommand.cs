using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.Categories.Commands.CreateCategory;

/// <summary>
/// Command to create a new category.
/// </summary>
public sealed record CreateCategoryCommand(
    string Name,
    string? Description = null,
    int DisplayOrder = 0) : ICommand<Guid>;
