using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.Categories.Commands.UpdateCategory;

/// <summary>
/// Command to update an existing category.
/// </summary>
public sealed record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description = null,
    int? DisplayOrder = null) : ICommand;
