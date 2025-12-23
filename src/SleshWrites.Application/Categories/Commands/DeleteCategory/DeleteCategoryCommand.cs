using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.Categories.Commands.DeleteCategory;

/// <summary>
/// Command to delete a category.
/// </summary>
public sealed record DeleteCategoryCommand(Guid Id) : ICommand;
