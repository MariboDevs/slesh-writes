using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.Authors.Commands.CreateAuthor;

/// <summary>
/// Command to create a new author.
/// </summary>
public sealed record CreateAuthorCommand(
    string AzureAdB2CId,
    string DisplayName,
    string? Email = null) : ICommand<Guid>;
