using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.Tags.Commands.CreateTag;

/// <summary>
/// Command to create a new tag.
/// </summary>
public sealed record CreateTagCommand(string Name) : ICommand<Guid>;
