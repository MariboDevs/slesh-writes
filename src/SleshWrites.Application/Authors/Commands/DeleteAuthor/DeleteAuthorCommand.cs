using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.Authors.Commands.DeleteAuthor;

/// <summary>
/// Command to delete an author.
/// </summary>
public sealed record DeleteAuthorCommand(Guid Id) : ICommand;
