using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.Authors.Commands.UpdateAuthor;

/// <summary>
/// Command to update an existing author.
/// </summary>
public sealed record UpdateAuthorCommand(
    Guid Id,
    string DisplayName,
    string? Bio = null,
    string? AvatarUrl = null) : ICommand;
